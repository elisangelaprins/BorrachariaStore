// Scripts do lado do cliente do BorrachariaStore.
// Ex.: máscaras de formulário, confirmações de exclusão (Aula 7), validações extras.

document.addEventListener("DOMContentLoaded", function () {
    console.log("BorrachariaStore carregado.");

    const formsAdicionar = document.querySelectorAll('form[action*="handler=AdicionarAoCarrinho"]');

    formsAdicionar.forEach(form => {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();

            try {
                const url = new URL(form.action, window.location.origin);
                const produtoId = url.searchParams.get("produtoId");

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
