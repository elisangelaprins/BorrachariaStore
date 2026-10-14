// Scripts do lado do cliente do BorrachariaStore.
// Ex.: máscaras de formulário, confirmações de exclusãoy6y, validações extras.

document.addEventListener("DOMContentLoaded", function () {
    console.log("BorrachariaStore carregado.");

    const formsAdicionar = document.querySelectorAll('form[action*="handler=AdicionarAoCarrinho"]');

        formsAdicionar.forEach(form => {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();

            try {
                const url = new URL(form.action, window.location.origin);
                const formData = new FormData(form);
                const produtoId = formData.get("produtoId") || url.searchParams.get("produtoId");

                if (!produtoId) {
                    form.submit();
                    return;
                }

                const response = await fetch(`/api/carrinho/adicionar?produtoId=${encodeURIComponent(produtoId)}`, {
                    method: "POST",
                    headers: {
                        "Accept": "application/json"
                    }
                });

                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {

                        atualizarContadorCarrinho(data.totalItens);

                        exibirToastFlutuante(data.nomeProduto);
                    }
                }
            } catch (error) {

                form.submit();
            }
        });
    });

    const slides = document.querySelectorAll(".hero-slide");
    const dots = document.querySelectorAll(".carousel-dots .dot");
    const prevBtn = document.getElementById("heroPrevBtn");
    const nextBtn = document.getElementById("heroNextBtn");
    let currentIndex = 0;
    let autoInterval;

    function showSlide(index) {
        if (slides.length === 0) return;
        currentIndex = (index + slides.length) % slides.length;
        slides.forEach((slide, i) => slide.classList.toggle("active", i === currentIndex));
        dots.forEach((dot, i) => dot.classList.toggle("active", i === currentIndex));
        const container = document.querySelector(".hero-carousel");
        if (container) container.style.transform = `translateX(-${currentIndex * 100}%)`;
    }

    function startAutoPlay() {
        stopAutoPlay();
        autoInterval = setInterval(() => showSlide(currentIndex + 1), 4500);
    }

    function stopAutoPlay() {
        if (autoInterval) clearInterval(autoInterval);
    }

    if (prevBtn) prevBtn.addEventListener("click", () => { showSlide(currentIndex - 1); startAutoPlay(); });
    if (nextBtn) nextBtn.addEventListener("click", () => { showSlide(currentIndex + 1); startAutoPlay(); });
    dots.forEach((dot, i) => dot.addEventListener("click", () => { showSlide(i); startAutoPlay(); }));

    if (slides.length > 0) startAutoPlay();
});

function atualizarContadorCarrinho(total) {
    const cartWrap = document.querySelector(".cart-wrap");
    if (!cartWrap) return;

    let badge = cartWrap.querySelector(".cart-badge");
    if (!badge) {
        badge = document.createElement("span");
        badge.className = "cart-badge";
        cartWrap.appendChild(badge);
    }
    badge.textContent = total;
}

function exibirToastFlutuante(nomeProduto) {
    const toastExistente = document.querySelector(".toast-success-banner");
    if (toastExistente) toastExistente.remove();

    const toast = document.createElement("div");
    toast.className = "toast-success-banner";
    toast.innerHTML = `
        <div class="toast-message">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <polyline points="20 6 9 17 4 12" />
            </svg>
            <span>"${nomeProduto}" foi adicionado ao seu carrinho!</span>
        </div>
        <a href="/Carrinho" class="toast-btn">
            Ver Carrinho
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M5 12h14" />
                <path d="m12 5 7 7-7 7" />
            </svg>
        </a>
    `;

    document.body.appendChild(toast);

    setTimeout(() => {
        toast.style.transition = "opacity 0.4s ease, transform 0.4s ease";
        toast.style.opacity = "0";
        toast.style.transform = "translateY(20px)";
        setTimeout(() => toast.remove(), 400);
    }, 4000);
}

//Gaveta detalhes produto
let fotosAtuaisGaveta = [];
let indiceFotoAtualGaveta = 0;

function abrirDetalhes(el) {
    var card = el.closest ? (el.closest('.catalogo-card') || el.closest('.destaque-card')) : el;
    if (!card) return;

    var d = card.dataset;
    document.getElementById('detalheMarca').textContent = d.marca || '';
    document.getElementById('detalheNome').textContent = d.nome || '';

    fotosAtuaisGaveta = [];
    if (d.fotos) {
        try {
            fotosAtuaisGaveta = JSON.parse(d.fotos);
        } catch (e) {
            fotosAtuaisGaveta = [];
        }
    }
    if (!fotosAtuaisGaveta || fotosAtuaisGaveta.length === 0) {
        fotosAtuaisGaveta = [d.urlFoto || '/images/logo/logo-icon.png'];
    }
    indiceFotoAtualGaveta = 0;
    renderizarGaleriaGaveta();

    var badgePromoEl = document.getElementById('detalheBadgePromo');
    if (d.badgePromo) {
        badgePromoEl.textContent = d.badgePromo;
        badgePromoEl.style.display = 'inline-block';
    } else {
        badgePromoEl.style.display = 'none';
    }

    document.getElementById('detalheBadgeAro').textContent = d.aro ? 'Aro ' + d.aro : '';
    document.getElementById('detalheAro').textContent = d.aro ? 'Aro ' + d.aro : '-';
    document.getElementById('detalheMedida').textContent = d.medida || '-';
    document.getElementById('detalheCategoria').textContent = d.categoria || '-';
    document.getElementById('detalheEstoque').textContent = (d.estoque && parseInt(d.estoque) > 0) ? d.estoque + ' unidades' : 'Em estoque';
    document.getElementById('detalheDescricao').textContent = d.descricao || 'Sem descrição detalhada disponível.';

    var oldPriceEl = document.getElementById('detalhePrecoAntigo');
    if (d.precoOriginal) {
        oldPriceEl.textContent = 'R$ ' + d.precoOriginal;
        oldPriceEl.style.display = 'block';
    } else {
        oldPriceEl.style.display = 'none';
    }

    document.getElementById('detalhePreco').textContent = 'R$ ' + (d.preco || '0,00');

    var valorNum = parseFloat((d.preco || '0').replace(/\./g, '').replace(',', '.'));
    if (!isNaN(valorNum) && valorNum > 0) {
        var parcela = (valorNum / 10).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        document.getElementById('detalheParcelas').textContent = 'ou 10x de R$ ' + parcela + ' sem juros';
    } else {
        document.getElementById('detalheParcelas').textContent = '';
    }

    document.getElementById('detalheProdutoId').value = d.id || '';
    var formGaveta = document.getElementById('detalhesAddForm');
    if (formGaveta) {
        formGaveta.action = '/Catalogo?handler=AdicionarAoCarrinho&produtoId=' + encodeURIComponent(d.id || '');
    }

    document.getElementById('detalhesOverlay').classList.add('open');
    document.getElementById('detalhesDrawer').classList.add('open');
    document.body.style.overflow = 'hidden';
}

function renderizarGaleriaGaveta() {
    var imgEl = document.getElementById('detalheImg');
    var prevBtn = document.getElementById('detalheGalleryPrev');
    var nextBtn = document.getElementById('detalheGalleryNext');
    var thumbsEl = document.getElementById('detalhesThumbs');

    if (!imgEl) return;

    var url = fotosAtuaisGaveta[indiceFotoAtualGaveta] || '/images/logo/logo-icon.png';
    imgEl.src = url;

    if (fotosAtuaisGaveta.length > 1) {
        if (prevBtn) prevBtn.style.display = 'flex';
        if (nextBtn) nextBtn.style.display = 'flex';
        if (thumbsEl) {
            thumbsEl.style.display = 'flex';
            thumbsEl.innerHTML = '';
            fotosAtuaisGaveta.forEach(function (f, idx) {
                var btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'detalhes-thumb-item' + (idx === indiceFotoAtualGaveta ? ' active' : '');
                btn.onclick = function () {
                    indiceFotoAtualGaveta = idx;
                    renderizarGaleriaGaveta();
                };
                var thumbImg = document.createElement('img');
                thumbImg.src = f;
                thumbImg.alt = '';
                thumbImg.onerror = function () { this.src = '/images/logo/logo-icon.png'; };
                btn.appendChild(thumbImg);
                thumbsEl.appendChild(btn);
            });
        }
    } else {
        if (prevBtn) prevBtn.style.display = 'none';
        if (nextBtn) nextBtn.style.display = 'none';
        if (thumbsEl) {
            thumbsEl.style.display = 'none';
            thumbsEl.innerHTML = '';
        }
    }
}

function mudarFotoDetalhes(delta) {
    if (fotosAtuaisGaveta.length <= 1) return;
    indiceFotoAtualGaveta = (indiceFotoAtualGaveta + delta + fotosAtuaisGaveta.length) % fotosAtuaisGaveta.length;
    renderizarGaleriaGaveta();
}

function fecharDetalhes() {
    var overlay = document.getElementById('detalhesOverlay');
    var drawer = document.getElementById('detalhesDrawer');
    if (overlay) overlay.classList.remove('open');
    if (drawer) drawer.classList.remove('open');
    document.body.style.overflow = '';
}

document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        fecharDetalhes();
    }
});

