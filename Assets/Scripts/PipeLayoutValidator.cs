using UnityEngine;

internal sealed class PipeLayoutValidator
{
    private readonly float minimumVisiblePipeHeight;
    private readonly float minimumRouteHeight;
    private readonly float closeSpacingThreshold;
    private readonly float minimumCloseRouteOverlap;

    public PipeLayoutValidator(
        float minVisiblePipeHeight,
        float minRouteHeight,
        float closeSpacing,
        float minCloseRouteOverlap)
    {
        minimumVisiblePipeHeight = minVisiblePipeHeight;
        minimumRouteHeight = minRouteHeight;
        closeSpacingThreshold = closeSpacing;
        minimumCloseRouteOverlap = minCloseRouteOverlap;
    }

    public bool IsPlayable(
        PipeLayout layout,
        float spacingFromPrevious,
        bool hasPreviousLayout,
        float previousRouteBottom,
        float previousRouteTop)
    {
        if (!HasValidPipeLengths(layout) || !HasValidRouteHeight(layout))
        {
            return false;
        }

        if (!hasPreviousLayout || spacingFromPrevious >= closeSpacingThreshold)
        {
            return true;
        }

        return GetRouteOverlap(
            previousRouteBottom,
            previousRouteTop,
            layout.RouteBottom,
            layout.RouteTop) >= minimumCloseRouteOverlap;
    }

    private bool HasValidPipeLengths(PipeLayout layout)
    {
        if (layout.HasBottomPipe
            && layout.BottomPipeLength < minimumVisiblePipeHeight)
        {
            return false;
        }

        return !layout.HasTopPipe
            || layout.TopPipeLength >= minimumVisiblePipeHeight;
    }

    private bool HasValidRouteHeight(PipeLayout layout)
    {
        return layout.RouteTop - layout.RouteBottom >= minimumRouteHeight;
    }

    private static float GetRouteOverlap(
        float firstBottom,
        float firstTop,
        float secondBottom,
        float secondTop)
    {
        float sharedBottom = Mathf.Max(firstBottom, secondBottom);
        float sharedTop = Mathf.Min(firstTop, secondTop);
        return sharedTop - sharedBottom;
    }
}
