/**
 * Returns a Chart.js config object for the given type and display options.
 *
 * @param {object} params
 * @param {'pie'|'bar'|'line'|'doughnut'} params.type          - Chart type
 * @param {string} params.cutout                                - Chart cutout (as a string like '70%')
 * @param {'top'|'bottom'|'left'|'right'|false} params.legend  - Legend position, or false to hide
 * @param {string}  [params.title]                              - Optional chart title
 * @param {boolean} [params.responsive]                         - Defaults to true
 * @returns {object} Chart.js config skeleton (no data yet)
 */
export function getConfig({ type = 'doughnut', cutout = '70%', legend = 'bottom', title = '', responsive = true } = {}) {
    return {
        type,
        data: { labels: [], datasets: [] },
        options: {
            responsive,
            cutout: cutout,
            plugins: {
                legend: legend
                    ? { display: true, position: legend, labels: { padding: 14, font: { size: 13 } } }
                    : { display: false },
                title: title
                    ? { display: true, text: title, font: { size: 16, weight: '500' }, padding: { bottom: 12 } }
                    : { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => ` ${ctx.label}: ${ctx.formattedValue}`
                    }
                },
                colors: {
                    enabled: true
                }

            }
        }
    };
}

/**
 * Creates a new chart on the canvas, or replaces an existing one.
 * Returns the new Chart.js instance — store it if you need to upsert again later.
 *
 * @param {HTMLCanvasElement} canvas
 * @param {object} params
 * @param {object}   params.config        - Chart.js config from getConfig()
 * @param {string[]} params.labels        - Data labels
 * @param {number[]} params.data          - Data values
 * @param {string[]} [params.colors]      - Background colors — falls back to PALETTE then auto HSL
 * @param {string}   [params.datasetLabel] - Dataset label (shown in bar/line tooltips)
 * @param {Chart}    [params.previous]    - Previous Chart.js instance to destroy before re-creating
 * @returns {Chart} The new Chart.js instance
 */
export function upsert(canvas, { config, labels, data, colors, datasetLabel = '', previous = null } = {}) {
    if (!canvas) throw new Error('upsert() requires a canvas element as its first argument');
    if (!config) throw new Error('upsert() requires a config object — use getConfig() to create one');

    if (previous) previous.destroy();
    let dataset = [{
        label: datasetLabel,
        data,
        borderWidth: 2,
    }];

    if (colors && colors.length > 0) {
        dataset[0].backgroundColor = colors;
    }
    console.log(dataset)
    return new Chart(canvas, {
        ...config,
        data: {
            labels,
            datasets: dataset
        }
    });
}

/**
 * updates the data of the first dataset to a given list of labels and data
 * data and labels should be the same size to prevent errors
 * @param {Chart} chart
 * @param {string[]} labels               - labels for the dataset
 * @param {[]}   data                     - data for the dataset
 */
export function changeSingularDataSetData(chart, labels, data) {
    if (labels && data && labels.length != data.length) throw new Error('data and labels not the same length');
    if (labels) {
        chart.data.labels = labels
    }
    if (data) {
        if (data.length != chart.data.labels.length) throw new Error('data and labels not the same length');
        const dataset = chart.data.datasets.find(() => true);
        if (dataset) {
            dataset.data = data;
        }

    }
    chart.update();
}

export function destroyChart(chart) {
    chart?.destroy();
}