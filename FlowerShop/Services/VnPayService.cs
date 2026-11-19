using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using FlowerShop.Models;

namespace FlowerShop.Services
{
    public class VnPayService
    {
        private static readonly Random Rng = new Random();
        private static readonly object RngLock = new object();

        private static int NextRandom8Digits()
        {
            lock (RngLock)
            {
                return Rng.Next(10000000, 99999999);
            }
        }

        public string CreatePaymentUrl(Order order, HttpRequestBase request)
        {
            string tmnCode = ConfigurationManager.AppSettings["VnPay:TmnCode"] ?? "CHAMFL01";
            string hashSecret = ConfigurationManager.AppSettings["VnPay:HashSecret"] ?? "CHAMFLOWERSVNPAYSECRETKEY2026SECURE";
            bool useLocalGateway = !string.Equals(ConfigurationManager.AppSettings["VnPay:UseLocalSandboxSimulator"], "false", StringComparison.OrdinalIgnoreCase);

            string scheme = request.Url != null ? request.Url.Scheme : "http";
            string host = request.Url != null ? request.Url.Authority : "localhost:5188";
            string returnUrl = string.Format("{0}://{1}/Payment/VnPayReturn", scheme, host);
            string baseUrl = useLocalGateway
                ? string.Format("{0}://{1}/Payment/VnPayGateway", scheme, host)
                : (ConfigurationManager.AppSettings["VnPay:BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html");

            var vnpParams = new SortedList<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((long)(order.Total * 100)).ToString(CultureInfo.InvariantCulture) },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", !string.IsNullOrEmpty(request.UserHostAddress) ? request.UserHostAddress : "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", string.Format("Thanh toan don hang hoa tuoi CHAM Flowers #{0}", order.Id) },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", order.Id.ToString(CultureInfo.InvariantCulture) }
            };

            var queryBuilder = new StringBuilder();
            foreach (var kv in vnpParams)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    queryBuilder.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string signData = queryBuilder.ToString().TrimEnd('&');
            string secureHash = HmacSha512(hashSecret, signData);

            return string.Format("{0}?{1}&vnp_SecureHash={2}", baseUrl, signData, secureHash);
        }

        public string CreateSimulatedCallbackUrl(NameValueCollection gatewayQuery, bool success, HttpRequestBase request)
        {
            string hashSecret = ConfigurationManager.AppSettings["VnPay:HashSecret"] ?? "CHAMFLOWERSVNPAYSECRETKEY2026SECURE";
            string txnRef = gatewayQuery["vnp_TxnRef"] ?? string.Empty;
            string amount = gatewayQuery["vnp_Amount"] ?? string.Empty;
            string orderInfo = gatewayQuery["vnp_OrderInfo"] ?? string.Empty;
            string tmnCode = gatewayQuery["vnp_TmnCode"] ?? string.Empty;

            var callbackParams = new SortedList<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Amount", amount },
                { "vnp_BankCode", "NCB" },
                { "vnp_BankTranNo", "VNP" + DateTime.Now.ToString("MMddHHmmss") },
                { "vnp_CardType", "ATM" },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_PayDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_ResponseCode", success ? "00" : "24" },
                { "vnp_TmnCode", string.IsNullOrEmpty(tmnCode) ? "CHAMFL01" : tmnCode },
                { "vnp_TransactionNo", NextRandom8Digits().ToString(CultureInfo.InvariantCulture) },
                { "vnp_TransactionStatus", success ? "00" : "02" },
                { "vnp_TxnRef", txnRef }
            };

            var sb = new StringBuilder();
            foreach (var kv in callbackParams)
            {
                sb.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            string signData = sb.ToString().TrimEnd('&');
            string secureHash = HmacSha512(hashSecret, signData);

            string scheme = request.Url != null ? request.Url.Scheme : "http";
            string host = request.Url != null ? request.Url.Authority : "localhost:5188";
            return string.Format("{0}://{1}/Payment/VnPayReturn?{2}&vnp_SecureHash={3}", scheme, host, signData, secureHash);
        }

        public bool ValidateSignature(NameValueCollection query)
        {
            string hashSecret = ConfigurationManager.AppSettings["VnPay:HashSecret"] ?? "CHAMFLOWERSVNPAYSECRETKEY2026SECURE";
            string inputHash = query["vnp_SecureHash"] ?? string.Empty;
            if (string.IsNullOrEmpty(inputHash))
                return false;

            var sorted = new SortedList<string, string>(StringComparer.Ordinal);
            foreach (string key in query.AllKeys)
            {
                if (!string.IsNullOrEmpty(key) &&
                    key.StartsWith("vnp_", StringComparison.Ordinal) &&
                    key != "vnp_SecureHash" &&
                    key != "vnp_SecureHashType")
                {
                    sorted[key] = query[key] ?? string.Empty;
                }
            }

            var sb = new StringBuilder();
            foreach (var kv in sorted)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    sb.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string signData = sb.ToString().TrimEnd('&');
            string expectedHash = HmacSha512(hashSecret, signData);

            return string.Equals(inputHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        public static string HmacSha512(string key, string inputData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                return BitConverter.ToString(hashValue).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
