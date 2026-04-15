const searchInput = document.getElementById('searchInput');
const sessionCards = Array.from(document.querySelectorAll('.session-card'));
const paginationButtons = document.getElementById('paginationButtons');
const noResultsMessage = document.getElementById('noResultsMessage');

const itemsPerPage = 5;
let currentPage = 1;
const range = 2;
let filteredCards = [...sessionCards];

function render() {
    const start = (currentPage - 1) * itemsPerPage;
    const end = start + itemsPerPage;

    sessionCards.forEach(card => card.style.setProperty('display', 'none', 'important'));

    const pageItems = filteredCards.slice(start, end);
    pageItems.forEach(card => card.style.setProperty('display', 'flex', 'important'));

    if (filteredCards.length === 0) {
        noResultsMessage.classList.remove('d-none');
    } else {
        noResultsMessage.classList.add('d-none');
    }

    updatePagination();
}

function updatePagination() {
    paginationButtons.innerHTML = '';
    const totalPages = Math.ceil(filteredCards.length / itemsPerPage);

    if (totalPages <= 1) return;

    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
    prevLi.innerHTML = `
                    <button class="page-link border-0 bg-transparent text-dark d-flex align-items-center gap-2" 
                            ${currentPage === 1 ? 'disabled' : ''} style="opacity: ${currentPage === 1 ? '0.5' : '1'}">
                        <i class="bi bi-arrow-left"></i> Previous
                    </button>`;
    prevLi.onclick = () => { if (currentPage > 1) { currentPage--; render(); window.scrollTo(0, 0); } };
    paginationButtons.appendChild(prevLi);

    for (let i = 1; i <= totalPages; i++) {
        if (i === 1 || i === totalPages || (i >= currentPage - range && i <= currentPage + range)) {
            if (i === totalPages && currentPage < totalPages - range - 1) {
                const dot = document.createElement('li');
                dot.className = 'page-item disabled';
                dot.innerHTML = '<span class="page-link border-0 bg-transparent text-dark">...</span>';
                paginationButtons.appendChild(dot);
            }

            const li = document.createElement('li');
            li.className = `page-item ${i === currentPage ? 'active' : ''}`;

            const btn = document.createElement('button');
            btn.className = 'page-link border-0 mx-1 rounded-3 fw-bold';
            btn.style.backgroundColor = i === currentPage ? '#333' : 'transparent';
            btn.style.color = i === currentPage ? '#fff' : '#333';
            btn.style.minWidth = '40px';
            btn.innerText = i;

            btn.onclick = () => { currentPage = i; render(); window.scrollTo(0, 0); };

            li.appendChild(btn);
            paginationButtons.appendChild(li);

            if (i === 1 && currentPage > range + 2) {
                const dot = document.createElement('li');
                dot.className = 'page-item disabled';
                dot.innerHTML = '<span class="page-link border-0 bg-transparent text-dark">...</span>';
                paginationButtons.appendChild(dot);
            }
        }
    }

    const nextLi = document.createElement('li');
    nextLi.className = `page-item ${currentPage === totalPages ? 'disabled' : ''}`;
    nextLi.innerHTML = `
                    <button class="page-link border-0 bg-transparent text-dark d-flex align-items-center gap-2" 
                            ${currentPage === totalPages ? 'disabled' : ''} style="opacity: ${currentPage === totalPages ? '0.5' : '1'}">
                        Next <i class="bi bi-arrow-right"></i>
                    </button>`;
    nextLi.onclick = () => { if (currentPage < totalPages) { currentPage++; render(); window.scrollTo(0, 0); } };
    paginationButtons.appendChild(nextLi);
}

searchInput.addEventListener('input', function (e) {
    const term = e.target.value.toLowerCase();

    filteredCards = sessionCards.filter(card => {
        const title = card.getAttribute('data-title');
        const speaker = card.getAttribute('data-speaker');
        return title.includes(term) || speaker.includes(term);
    });

    currentPage = 1;
    render();
});

render();