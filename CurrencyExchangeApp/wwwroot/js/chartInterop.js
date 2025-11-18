window.chartInterop = {
    chart: null,
    
    initializeChart: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        
        // Destroy existing chart if it exists
        if (this.chart) {
            this.chart.destroy();
        }
        
        // Create new chart
        this.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Exchange Rate',
                    data: data,
                    borderColor: '#4cc9f0',
                    backgroundColor: 'rgba(76, 201, 240, 0.1)',
                    borderWidth: 3,
                    pointBackgroundColor: '#4361ee',
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: {
                    duration: 1000,
                    easing: 'easeOutQuart'
                },
                scales: {
                    x: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#94a3b8'
                        }
                    },
                    y: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#94a3b8'
                        }
                    }
                },
                plugins: {
                    legend: {
                        labels: {
                            color: '#f1f5f9',
                            font: {
                                size: 14
                            }
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(15, 23, 42, 0.9)',
                        titleColor: '#4cc9f0',
                        bodyColor: '#f1f5f9',
                        borderColor: 'rgba(255, 255, 255, 0.18)',
                        borderWidth: 1,
                        padding: 12,
                        displayColors: false
                    }
                }
            }
        });
    },
    
    updateChart: function (labels, data) {
        if (this.chart) {
            this.chart.data.labels = labels;
            this.chart.data.datasets[0].data = data;
            this.chart.update('easeOutQuart', 750);
        }
    }
};