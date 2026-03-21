import json
from playwright.sync_api import sync_playwright

def test_mermaid_markdown_import_issequence():
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(headless=True)
        page = browser.new_page()
        page.goto("http://localhost:5165")

        page.wait_for_selector("text=SimplyMermaid", state="visible")

        # Open Import Drawer
        page.locator("button.mud-icon-button").nth(7).click()

        # Hybrid Markdown export
        mermaid_code = """
sequenceDiagram
    participant Node1 as "Node 1"
    participant Node2 as "Node 2"
    Node1->>Node2: Request ABC
    Node2-->>Node1: Response!!!
    participant Node4 as "Node 4"
    participant Node5 as "Node 5"
    participant Node6 as "Node 6"

%% SimplyMermaidPositions: {"Node1":[566.2,48.4,120,60,1],"Node2":[870.1,52.1,120,60,1],"Node4":[1018.1,9.2,120,60,0],"Node5":[1329.3,8.2,120,60,0],"Node6":[1167.1,182.5,120,60,0]}
        """

        # Ensure #mermaid-source-code is visible and fill it
        page.wait_for_selector("#mermaid-source-code", state="visible")
        page.locator("#mermaid-source-code").fill(mermaid_code.strip())
        page.locator("text=Import Mermaid Text").click(force=True)

        page.wait_for_timeout(1000)

        # Select "Sequence" diagram type from dropdown to ensure it renders sequence timelines
        page.locator(".mud-select").nth(1).click()
        page.locator("text=Sequence").nth(0).click()
        page.wait_for_timeout(500)

        # Count the number of timelines.
        # Node 1 and Node 2 are IsSequence=True and Type=0 (Rectangle) -> should have timelines.
        # Node 4, 5, 6 are IsSequence=False (0) -> should NOT have timelines.
        timeline_count = page.locator("g > line[stroke-width='30']").count()
        print(f"Number of timelines: {timeline_count}")
        assert timeline_count == 2, f"Expected 2 timelines, but found {timeline_count}."

        browser.close()
        print("Markdown import test passed successfully.")

if __name__ == "__main__":
    test_mermaid_markdown_import_issequence()
