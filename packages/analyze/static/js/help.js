function getHelpPageContent() {
    const contentTemplate = 
`
<div style="max-width: 800px">
    <ul>
        <li><a id="help-severity" class="help-route" href="#" loc-text="IssueSeverity">Issue Severity</a></li>
        <li><a id="help-storyPoints" class="help-route" href="#" loc-text="WhatAreStoryPoints">What are Story Points?</a></li>
    </ul>
</div>
`;

    return {
        "content": contentTemplate,
        "postAction": () => {
            $('.help-route').each(function() {
                $(this).on("click", function() {
                    showPage($(this).attr('id'), $(this));
                });
            });
        }
    };
}
