// Vista previa: expandir comprobantes y buscador.
(function () {
    const tabla = document.getElementById('tabla-comprobantes');
    if (!tabla) return;

    const filas = Array.from(tabla.querySelectorAll('tr.fila-comprobante'));
    const buscador = document.getElementById('buscador');
    const contador = document.getElementById('contador');
    const sinResultados = document.getElementById('sin-resultados');
    const total = filas.length;

    // Igual que Busqueda.Normalizar en el servidor: minúsculas y sin acentos.
    const normalizar = texto => texto.toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '').trim();

    const detalleDe = fila => fila.nextElementSibling;

    function expandir(fila, expandido) {
        fila.setAttribute('aria-expanded', String(expandido));
        fila.querySelector('.boton-expandir').textContent = expandido ? '▾' : '▸';
        detalleDe(fila).hidden = !expandido;
    }

    tabla.addEventListener('click', evento => {
        const fila = evento.target.closest('tr.fila-comprobante');
        if (!fila) return;
        expandir(fila, fila.getAttribute('aria-expanded') !== 'true');
    });

    function filtrar() {
        const consulta = normalizar(buscador.value);
        let visibles = 0;

        for (const fila of filas) {
            const coincide = consulta === '' || fila.dataset.busqueda.includes(consulta);
            fila.hidden = !coincide;
            detalleDe(fila).hidden = !coincide || fila.getAttribute('aria-expanded') !== 'true';
            if (coincide) visibles++;
        }

        sinResultados.hidden = visibles > 0;
        contador.textContent = `${visibles} de ${total} comprobantes`;
    }

    let espera;
    buscador.addEventListener('input', () => {
        clearTimeout(espera);
        espera = setTimeout(filtrar, 150);
    });
})();
