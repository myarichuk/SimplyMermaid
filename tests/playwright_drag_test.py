import json
import os
from playwright.sync_api import sync_playwright

def setup_page(browser):
    page = browser.new_page()
    page.goto("http://localhost:5165")
    page.wait_for_selector("text=SimplyMermaid", state="visible")
    return page

def test_drag_and_drop_shape(page):
    # Ensure canvas is clear
    page.locator("button:has(svg:has(path[d='M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z']))").nth(0).click(force=True)
    page.wait_for_timeout(500)
    assert page.locator("g.graph-node").count() == 0

    # Locate the first shape icon in the sidebar (Rectangle)
    shape_icon = page.locator("div.shape-icon").first
    canvas = page.locator("svg#pretty-chart")

    # Perform drag and drop from sidebar to canvas
    shape_icon.drag_to(canvas)
    page.wait_for_timeout(500)

    # Verify a node was added
    assert page.locator("g.graph-node").count() == 1, "Drag and drop failed: node count is not 1."
    print("Drag and drop shape works.")

def test_drag_pan_canvas(page):
    # Ensure canvas is clear
    page.locator("button:has(svg:has(path[d='M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z']))").nth(0).click(force=True)
    page.wait_for_timeout(500)

    # We can check if dragging changes the translate in the canvas inner g
    # Find the main transform group
    # The transform group is <g transform="translate(panX, panY) scale(zoom)">
    # It has no specific ID, but it's the first child <g> of #pretty-chart that contains the grid.
    # Actually, a better way is to evaluate the local panX/panY or just check the DOM if possible.
    # We will use mouse events to simulate a pan drag.

    canvas = page.locator("svg#pretty-chart")
    bounding_box = canvas.bounding_box()
    start_x = bounding_box['x'] + bounding_box['width'] / 2
    start_y = bounding_box['y'] + bounding_box['height'] / 2

    # Since pan state isn't exposed directly, we can drop a node, pan, and see if the node's visual position moved
    shape_icon = page.locator("div.shape-icon").first
    shape_icon.drag_to(canvas, target_position={"x": start_x, "y": start_y})
    page.wait_for_timeout(500)

    # Get node bounding box before pan
    node = page.locator("g.graph-node").first
    bbox_before = node.bounding_box()

    # Now simulate dragging empty canvas to pan
    # We must drag from an empty spot. So we click away from the node.
    empty_x = bounding_box['x'] + 50
    empty_y = bounding_box['y'] + 50

    page.mouse.move(empty_x, empty_y)
    page.mouse.down()
    page.mouse.move(empty_x + 100, empty_y + 100, steps=10)
    page.mouse.up()
    page.wait_for_timeout(500)

    bbox_after = node.bounding_box()

    assert bbox_before['x'] != bbox_after['x'] or bbox_before['y'] != bbox_after['y'], "Pan failed: node visual position did not change."
    print("Drag to pan canvas works.")

def test_all_scenarios():
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(headless=True)
        page = setup_page(browser)

        test_drag_and_drop_shape(page)
        test_drag_pan_canvas(page)

        browser.close()
        print("Drag and drop scenarios tested successfully.")

if __name__ == "__main__":
    test_all_scenarios()
