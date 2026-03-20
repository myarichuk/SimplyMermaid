namespace MermaidEditor.Services;

using MermaidEditor.Models;
using System.Text.Json;
using System;

public class SnapshotCommand : ICommand
{
    private readonly Graph _graphRef;
    private readonly string _stateBefore;
    private readonly string _stateAfter;
    private readonly Action _onStateRestored;

    public SnapshotCommand(Graph graphRef, string stateBefore, string stateAfter, Action onStateRestored)
    {
        _graphRef = graphRef;
        _stateBefore = stateBefore;
        _stateAfter = stateAfter;
        _onStateRestored = onStateRestored;
    }

    public void Execute()
    {
        RestoreState(_stateAfter);
    }

    public void Undo()
    {
        RestoreState(_stateBefore);
    }

    private void RestoreState(string state)
    {
        var restoredGraph = JsonSerializer.Deserialize<Graph>(state);
        if (restoredGraph != null)
        {
            _graphRef.Nodes.Clear();
            _graphRef.Nodes.AddRange(restoredGraph.Nodes);
            _graphRef.Edges.Clear();
            _graphRef.Edges.AddRange(restoredGraph.Edges);
            _onStateRestored?.Invoke();
        }
    }
}
