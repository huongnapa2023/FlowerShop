// CHAM Flowers - Client Interactions (AJAX Cart, HotDeal Popup, Countdown, SignalR 2 Live Chat)

function showNtToast(message, isSuccess) {
    if (typeof isSuccess === 'undefined') isSuccess = true;
    var container = document.getElementById('ntToastContainer');
    if (!container) return;

    var el = document.createElement('div');
    el.className = 'alert ' + (isSuccess ? 'alert-success border-success' : 'alert-danger border-danger') + ' shadow-lg d-flex align-items-center gap-2 py-2 px-3 mb-2 rounded-4';
    el.innerHTML = '<i class="bi ' + (isSuccess ? 'bi-check-circle-fill text-success' : 'bi-exclamation-triangle-fill text-danger') + ' fs-5"></i>' +
                   '<div class="small fw-semibold">' + message + '</div>';
    container.appendChild(el);
    setTimeout(function () {
        el.style.opacity = '0';
        el.style.transition = 'opacity 0.3s ease';
        setTimeout(function () { el.remove(); }, 320);
    }, 3200);
}

// Thêm giỏ hàng bằng AJAX không cần tải lại trang (Mục 3.1.3)
function addToCartAjax(productId, quantity) {
    if (!quantity) quantity = 1;
    $.ajax({
        url: '/Order/AddToCart',
        type: 'POST',
        data: { productId: productId, quantity: quantity },
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        success: function (res) {
            if (res && res.success) {
                showNtToast(res.message, true);
                var badge = document.getElementById('cartCountBadge');
                if (badge) {
                    badge.textContent = res.cartCount;
                    badge.classList.remove('d-none');
                }
            } else {
                showNtToast((res && res.message) || 'Không thể thêm vào giỏ hàng.', false);
            }
        },
        error: function () {
            showNtToast('Lỗi kết nối máy chủ khi thêm vào giỏ hàng.', false);
        }
    });
}

function refreshCartBadge() {
    var badge = document.getElementById('cartCountBadge');
    if (!badge) return;
    $.get('/Order/GetCartCount', function (res) {
        if (res && typeof res.count === 'number') {
            badge.textContent = res.count;
            if (res.count > 0) badge.classList.remove('d-none');
        }
    });
}

// Khởi tạo bộ đếm ngược Countdown cho Hot Deal (Mục 3.1.3)
function initCountdowns() {
    document.querySelectorAll('[data-countdown]').forEach(function (box) {
        var targetStr = box.getAttribute('data-countdown');
        if (!targetStr) return;
        var targetDate = new Date(targetStr).getTime();

        var tick = function () {
            var now = Date.now();
            var diff = Math.max(0, targetDate - now);
            var days = Math.floor(diff / (1000 * 60 * 60 * 24));
            diff -= days * (1000 * 60 * 60 * 24);
            var hours = Math.floor(diff / (1000 * 60 * 60));
            diff -= hours * (1000 * 60 * 60);
            var mins = Math.floor(diff / (1000 * 60));
            diff -= mins * (1000 * 60);
            var secs = Math.floor(diff / 1000);

            var dEl = box.querySelector('.cd-days');
            var hEl = box.querySelector('.cd-hours');
            var mEl = box.querySelector('.cd-mins');
            var sEl = box.querySelector('.cd-secs');
            if (dEl) dEl.textContent = String(days).padStart(2, '0');
            if (hEl) hEl.textContent = String(hours).padStart(2, '0');
            if (mEl) mEl.textContent = String(mins).padStart(2, '0');
            if (sEl) sEl.textContent = String(secs).padStart(2, '0');
        };
        tick();
        setInterval(tick, 1000);
    });
}

// Tải HotDealPopup động qua AJAX /HotDeal/GetActivePopup (Popup Builder nâng cao)
function checkHotDealPopup(forceOpen) {
    var modalEl = document.getElementById('hotDealPopupModal');
    if (!modalEl) return;

    $.get('/HotDeal/GetActivePopup', function (res) {
        if (!res || !res.hasPopup) return;

        var popId = res.id || 1;
        var freq = res.showFrequency || 'OncePerSession';
        var todayKey = new Date().toISOString().slice(0, 10);

        if (!forceOpen) {
            if (freq === 'OncePerSession' && sessionStorage.getItem('ntPopupShown_' + popId) === '1') {
                return;
            }
            if (freq === 'OncePerDay' && localStorage.getItem('ntPopupDay_' + popId) === todayKey) {
                return;
            }
        }

        var layout = res.layoutType || 'TopImage';
        var bg = res.backgroundColor || '#ffffff';
        var textCol = res.textColor || '#1e293b';
        var accentCol = res.accentColor || '#e11d48';
        var badge = res.badgeText || '';
        var title = res.title || 'Ưu đãi Hoa Tươi CHAM Flowers';
        var sub = res.subtitle || '';
        var coupon = res.couponCode || '';
        var btnText = res.buttonText || 'Xem ngay chương trình khuyến mãi';
        var url = res.productUrl || '/HotDeal';
        var img = res.imageUrl || '/images/samflora/b12-main.jpg';

        var dialogEl = document.getElementById('hdPopupDialog');
        if (dialogEl) {
            if (layout === 'SplitLeft') {
                dialogEl.classList.add('modal-lg');
            } else {
                dialogEl.classList.remove('modal-lg');
            }
        }

        var bodyEl = document.getElementById('hdPopupDynamicBody');
        if (bodyEl) {
            var badgeHtml = badge
                ? '<div class="mb-2"><span class="badge rounded-pill px-3 py-1 fw-bold" style="background:' + accentCol + ';color:#fff;font-size:0.74rem;letter-spacing:0.04em">' + escapeHtml(badge) + '</span></div>'
                : '';
            var couponHtml = coupon
                ? '<div class="mb-3"><span class="d-inline-block px-3 py-1 rounded-pill fw-bold small" style="border:1.5px dashed ' + accentCol + ';color:' + (layout === 'PosterOverlay' ? '#fff' : accentCol) + ';background:rgba(225,29,72,0.08)">Mã ưu đãi: <strong class="font-monospace">' + escapeHtml(coupon) + '</strong></span></div>'
                : '';
            var ctaHtml = '<a href="' + escapeHtml(url) + '" class="btn w-100 py-2 rounded-pill fw-bold shadow-sm text-white" style="background:' + accentCol + ';border:none">' + escapeHtml(btnText) + '</a>';

            if (layout === 'SplitLeft') {
                bodyEl.innerHTML =
                    '<div class="row g-0 align-items-stretch" style="background:' + bg + ';color:' + textCol + ';min-height:340px">' +
                        '<div class="col-md-5">' +
                            '<img src="' + escapeHtml(img) + '" class="w-100 h-100" style="object-fit:cover;min-height:240px" alt="' + escapeHtml(title) + '" />' +
                        '</div>' +
                        '<div class="col-md-7 p-4 p-lg-5 d-flex flex-column justify-content-center text-start">' +
                            badgeHtml +
                            '<h4 class="serif-heading fw-bold mb-2" style="color:' + accentCol + '">' + escapeHtml(title) + '</h4>' +
                            '<p class="small opacity-75 mb-3">' + escapeHtml(sub) + '</p>' +
                            couponHtml +
                            ctaHtml +
                        '</div>' +
                    '</div>';
            } else if (layout === 'PosterOverlay') {
                bodyEl.innerHTML =
                    '<div class="position-relative text-white d-flex flex-column justify-content-end p-4 text-center" style="min-height:410px;background:linear-gradient(180deg, rgba(15,23,42,0.12) 0%, rgba(15,23,42,0.88) 78%), url(\'' + escapeHtml(img) + '\') center/cover no-repeat;">' +
                        badgeHtml +
                        '<h3 class="serif-heading fw-bold mb-2 text-white">' + escapeHtml(title) + '</h3>' +
                        '<p class="small text-white-50 mb-3">' + escapeHtml(sub) + '</p>' +
                        couponHtml +
                        ctaHtml +
                    '</div>';
            } else {
                bodyEl.innerHTML =
                    '<div class="position-relative">' +
                        '<img src="' + escapeHtml(img) + '" class="w-100" style="height:255px;object-fit:cover;object-position:center 35%" alt="' + escapeHtml(title) + '" />' +
                    '</div>' +
                    '<div class="p-4 text-center" style="background:' + bg + ';color:' + textCol + '">' +
                        badgeHtml +
                        '<h4 class="serif-heading fw-bold mb-2" style="color:' + accentCol + '">' + escapeHtml(title) + '</h4>' +
                        '<p class="small opacity-75 mb-3">' + escapeHtml(sub) + '</p>' +
                        couponHtml +
                        ctaHtml +
                    '</div>';
            }
        }

        var openNow = function () {
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
            sessionStorage.setItem('ntPopupShown_' + popId, '1');
            localStorage.setItem('ntPopupDay_' + popId, todayKey);
        };

        var delayMs = forceOpen ? 0 : Math.max(0, (parseInt(res.delaySeconds, 10) || 0) * 1000);
        if (delayMs > 0) {
            setTimeout(openNow, delayMs);
        } else {
            openNow();
        }
    });
}

// ==================== CHAT HỖ TRỢ TRỰC TUYẾN (ASP.NET SIGNALR 2 + AJAX) ====================
var ntChatSessionKey = null;
var renderedMsgIds = new Set();

function escapeHtml(str) {
    if (!str) return '';
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

function renderChatBubble(msg, container) {
    if (!container || !msg) return;
    if (msg.id && renderedMsgIds.has(msg.id)) return;
    if (msg.id) renderedMsgIds.add(msg.id);

    var roleClass = msg.senderRole === 'Customer' ? 'customer'
        : (msg.senderRole === 'System' ? 'system' : 'admin');

    var extraHtml = '';
    if (msg.messageType === 'ProductLink' && msg.attachmentUrl) {
        extraHtml = '<div class="mt-2"><a href="' + msg.attachmentUrl + '" class="btn btn-sm btn-nt-outline py-1 px-2 small">Xem mẫu hoa ngay →</a></div>';
    } else if (msg.messageType === 'Coupon' && msg.attachmentUrl) {
        extraHtml = '<div class="mt-2 p-2 rounded-3 bg-warning-subtle text-dark small fw-bold">Mã ưu đãi: <span class="text-danger">' + msg.attachmentUrl + '</span></div>';
    } else if (msg.messageType === 'Image' && msg.attachmentUrl) {
        extraHtml = '<div class="mt-2"><img src="' + msg.attachmentUrl + '" class="img-fluid rounded-3" style="max-height:130px" alt="Ảnh hoa thực tế"/></div>';
    }

    var div = document.createElement('div');
    div.className = 'chat-bubble ' + roleClass;
    div.innerHTML = '<div>' + escapeHtml(msg.content) + '</div>' + extraHtml + '<div class="opacity-75 mt-1" style="font-size:0.68rem">' + (msg.createdAt || '') + '</div>';
    container.appendChild(div);
    container.scrollTop = container.scrollHeight;
}

function initCustomerChatWidget() {
    var launcher = document.getElementById('ntChatLauncher');
    var panel = document.getElementById('ntChatPanel');
    var closeBtn = document.getElementById('ntChatClose');
    var msgList = document.getElementById('ntChatMessages');
    var form = document.getElementById('ntChatForm');
    var input = document.getElementById('ntChatInput');

    if (!launcher || !panel) return;

    launcher.addEventListener('click', function () {
        panel.classList.toggle('open');
        if (panel.classList.contains('open')) {
            msgList.scrollTop = msgList.scrollHeight;
            if (input) input.focus();
        }
    });

    if (closeBtn) {
        closeBtn.addEventListener('click', function () { panel.classList.remove('open'); });
    }

    $.get('/Chat/InitSession', function (res) {
        ntChatSessionKey = res.sessionKey;
        if (Array.isArray(res.messages) && res.messages.length > 0) {
            msgList.innerHTML = '';
            res.messages.forEach(function (m) { renderChatBubble(m, msgList); });
        }

        if ($ && $.connection && $.connection.chatHub) {
            var hub = $.connection.chatHub;
            hub.client.receiveMessage = function (payload) {
                if (payload && payload.sessionKey === ntChatSessionKey) {
                    renderChatBubble(payload, msgList);
                    if (!panel.classList.contains('open') && payload.senderRole === 'Admin') {
                        showNtToast('CSKH CHAM Flowers vừa gửi tin nhắn mới cho bạn!', true);
                    }
                }
            };

            $.connection.hub.start().done(function () {
                hub.server.joinCustomerRoom(ntChatSessionKey);
            });
        }
    });

    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var text = input.value.trim();
            if (!text) return;
            input.value = '';

            $.post('/Chat/SendCustomerMessage', {
                sessionKey: ntChatSessionKey,
                content: text
            }, function (res) {
                if (res && res.success) {
                    renderChatBubble(res.message, msgList);
                } else {
                    showNtToast((res && res.message) || 'Gửi tin nhắn không thành công.', false);
                }
            }).fail(function () {
                showNtToast('Lỗi gửi tin nhắn, vui lòng thử lại.', false);
            });
        });
    }
}

document.addEventListener('DOMContentLoaded', function () {
    refreshCartBadge();
    initCountdowns();
    initCustomerChatWidget();

    // Global Event Delegation (Không cần viết inline onclick trong Razor view)
    document.addEventListener('click', function (e) {
        var cartBtn = e.target.closest('.js-add-to-cart');
        if (cartBtn) {
            e.preventDefault();
            var pid = cartBtn.getAttribute('data-id');
            var qtyInput = document.getElementById('buyQty');
            var qty = qtyInput ? (parseInt(qtyInput.value, 10) || 1) : 1;
            addToCartAjax(pid, qty);
            return;
        }

        var hotDealBtn = e.target.closest('.js-open-hotdeal');
        if (hotDealBtn) {
            e.preventDefault();
            checkHotDealPopup(true);
            return;
        }

        var qtyBtn = e.target.closest('.js-qty-btn');
        if (qtyBtn) {
            e.preventDefault();
            var step = parseInt(qtyBtn.getAttribute('data-step'), 10) || 0;
            var qInput = document.getElementById('buyQty');
            if (qInput) {
                var cur = parseInt(qInput.value, 10) || 1;
                var max = parseInt(qInput.getAttribute('max'), 10) || 999;
                var min = parseInt(qInput.getAttribute('min'), 10) || 1;
                var next = cur + step;
                if (next >= min && next <= max) {
                    qInput.value = next;
                }
            }
            return;
        }
    });
});
