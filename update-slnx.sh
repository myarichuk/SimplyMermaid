#!/bin/bash
sed -i 's#</Solution>#  <Folder Name="/src/">\n    <Project Path="src/MermaidEditor.Maui/MermaidEditor.Maui.csproj" />\n  </Folder>\n</Solution>#' MermaidEditor.slnx
