function getDashboardPageContent() {

    const dashboardContentTemplate = 
`
<div class="row my-4">
    <div class="col-12 col-xl-4 col-lg-4 col-md-6 mb-4 mb-lg-0">
        <div class="card mb-4">
            <div id="card-summary" class="card-body" style="min-width: 230px">
            </div>
        </div>
        
        <div class="card">
            <div id="card-severity-chart" class="card-body">
                <p class="card-title card-title-upper card-title-bold" loc-text="Severity">severity</p>
                <canvas id="severity-chart"></canvas>
            </div>
        </div>
    </div>

    <div class="col-12 col-xl-8 col-lg-8 col-md-6 mb-4 mb-lg-0">
        <div class="card">
            <div id="card-categories-chart" class="card-body">
                <p class="card-title card-title-upper card-title-bold" loc-text="Categories">categories</p>
                <canvas id="category-chart"></canvas>
            </div>
        </div>
    </div>
</div>
`;

    let html = $(dashboardContentTemplate);

    html.find('#card-summary').html(generateAggregateSummaryCard());

    return {
        "content" : html,
        "postAction": () => {
            generateSeverityCard(results.stats.charts.severity, $('#severity-chart')[0]);
            generateCategoryCard(results.stats.charts.category, $('#category-chart')[0]);

            $('#route-inline-help-issues').on("click", function() {
                showPage("help-issues");
            });

            $('#route-inline-help-incidents').on("click", function() {
                showPage("help-incidents");
            });

            $('#route-inline-help-storyPoints').on("click", function() {
                showPage("help-storyPoints");
            });
        }
    };
}

function generateAggregateSummaryCard() {
    let cardHTML = '<p class="card-title card-title-upper card-title-bold" loc-text="summary">summary</p>';

    let projectCount = results.stats.summary.projects !== undefined ? results.stats.summary.projects : 0;
    let issuesCount = results.stats.summary.issues !== undefined ? results.stats.summary.issues : 0;
    let incidentsCount = results.stats.summary.incidents !== undefined ? results.stats.summary.incidents : 0;
    let storyPointsCount = results.stats.summary.effort !== undefined ? results.stats.summary.effort : 0;

    cardHTML += 
        `<div>
            <h3 class="card-text card-main-content">${projectCount}</h3>
            <span id="SummaryReportEntries" class="card-text" loc-text="projects">projects</span>
        </div>
        <div>
            <h3 class="card-text card-main-content">${issuesCount}</h3>
            <span class="card-text" loc-text="issues">issues</span>
            <a id="route-inline-help-issues" href="#" loc-title="WhatAreIssues" title="What are issues?">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="feather feather-help-circle">
                <circle cx="12" cy="12" r="10"></circle>
                <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                <line x1="12" y1="17" x2="12.01" y2="17"></line>
            </svg>
        </a>
        </div>
        <div>
            <h3 class="card-text card-main-content">${incidentsCount}</h3>
            <span class="card-text" loc-text="incidents">incidents</span>
            <a id="route-inline-help-incidents" href="#" loc-title="WhatAreIncidents" title="What are incidents?">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="feather feather-help-circle">
                    <circle cx="12" cy="12" r="10"></circle>
                    <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                    <line x1="12" y1="17" x2="12.01" y2="17"></line>
                </svg>
            </a>
        </div>
        <div>
            <h3 class="card-text card-main-content">${storyPointsCount}</h3>
            <span class="card-text" loc-text="storyPoints">story points</span>
            <a id="route-inline-help-storyPoints" href="#" loc-title="WhatAreStoryPoints" title="What are Story Points?">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="feather feather-help-circle">
                    <circle cx="12" cy="12" r="10"></circle>
                    <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                    <line x1="12" y1="17" x2="12.01" y2="17"></line>
                </svg>
            </a>
        </div>`;

    return cardHTML;
}
