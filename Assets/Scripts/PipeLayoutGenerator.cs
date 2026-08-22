using System;
using UnityEngine;
using Random = UnityEngine.Random;

internal enum PipeObstacleType
{
    StandardPair,
    BottomOnly,
    TopOnly,
    AsymmetricPair
}

internal struct PipeLayout
{
    public PipeObstacleType Type;
    public bool HasTopPipe;
    public bool HasBottomPipe;
    public float TopPipeLength;
    public float BottomPipeLength;
    public float RouteBottom;
    public float RouteTop;
}

internal sealed class PipeLayoutGenerator
{
    private const float StandardFallbackGapSize = 3.2f;

    private readonly float standardPairWeight;
    private readonly float bottomOnlyWeight;
    private readonly float topOnlyWeight;
    private readonly float asymmetricPairWeight;
    private readonly float minimumGapCenter;
    private readonly float maximumGapCenter;
    private readonly float minimumGapSize;
    private readonly float maximumGapSize;
    private readonly float minimumPipeLength;
    private readonly float maximumPipeLength;
    private readonly float minimumStandardPipeLength;
    private readonly float groundTopY;
    private readonly float lowerPipeExtent;
    private readonly float upperPipeExtent;

    public PipeLayoutGenerator(
        float standardWeight,
        float bottomWeight,
        float topWeight,
        float asymmetricWeight,
        float minGapCenter,
        float maxGapCenter,
        float minGapSize,
        float maxGapSize,
        float minPipeLength,
        float maxPipeLength,
        float minStandardPipeLength,
        float groundTop,
        float lowerExtent,
        float upperExtent)
    {
        standardPairWeight = standardWeight;
        bottomOnlyWeight = bottomWeight;
        topOnlyWeight = topWeight;
        asymmetricPairWeight = asymmetricWeight;
        minimumGapCenter = minGapCenter;
        maximumGapCenter = maxGapCenter;
        minimumGapSize = minGapSize;
        maximumGapSize = maxGapSize;
        minimumPipeLength = minPipeLength;
        maximumPipeLength = maxPipeLength;
        minimumStandardPipeLength = minStandardPipeLength;
        groundTopY = groundTop;
        lowerPipeExtent = lowerExtent;
        upperPipeExtent = upperExtent;
    }

    public PipeObstacleType ChooseObstacleType()
    {
        float totalWeight = standardPairWeight + bottomOnlyWeight
            + topOnlyWeight + asymmetricPairWeight;
        float choice = Random.Range(0f, totalWeight);

        if (choice < standardPairWeight)
        {
            return PipeObstacleType.StandardPair;
        }

        choice -= standardPairWeight;
        if (choice < bottomOnlyWeight)
        {
            return PipeObstacleType.BottomOnly;
        }

        choice -= bottomOnlyWeight;
        if (choice < topOnlyWeight)
        {
            return PipeObstacleType.TopOnly;
        }

        return PipeObstacleType.AsymmetricPair;
    }

    public PipeLayout CreateRandomLayout(PipeObstacleType type)
    {
        switch (type)
        {
            case PipeObstacleType.BottomOnly:
                return CreateBottomOnlyLayout(
                    Random.Range(minimumPipeLength, maximumPipeLength));

            case PipeObstacleType.TopOnly:
                return CreateTopOnlyLayout(
                    Random.Range(minimumPipeLength, maximumPipeLength));

            case PipeObstacleType.AsymmetricPair:
                float topLength = Random.Range(minimumPipeLength, maximumPipeLength);
                float bottomLength = Random.Range(minimumPipeLength, maximumPipeLength);
                return CreateAsymmetricLayout(topLength, bottomLength);

            default:
                return CreateRandomStandardLayout();
        }
    }

    public PipeLayout CreateSafeFallbackLayout(
        PipeObstacleType type,
        bool mustOverlapPrevious,
        float previousRouteBottom,
        float previousRouteTop)
    {
        if (!mustOverlapPrevious)
        {
            return CreateOriginalFallbackLayout(type);
        }

        switch (type)
        {
            case PipeObstacleType.BottomOnly:
                return CreateBottomOnlyLayout(minimumStandardPipeLength);

            case PipeObstacleType.TopOnly:
                return CreateTopOnlyLayout(minimumStandardPipeLength);

            case PipeObstacleType.AsymmetricPair:
                return CreateAsymmetricLayout(
                    minimumStandardPipeLength,
                    minimumStandardPipeLength);

            default:
                return CreateOverlappingStandardLayout(
                    previousRouteBottom,
                    previousRouteTop);
        }
    }

    public PipeLayout CreateEmergencyFallbackLayout()
    {
        float maximumOpenBottom = lowerPipeExtent + minimumStandardPipeLength;
        float maximumOpenTop = upperPipeExtent - minimumStandardPipeLength;

        if (maximumOpenTop <= maximumOpenBottom)
        {
            throw new InvalidOperationException(
                "Pipe configuration leaves no room for a playable fallback route.");
        }

        return new PipeLayout
        {
            Type = PipeObstacleType.StandardPair,
            HasTopPipe = true,
            HasBottomPipe = true,
            TopPipeLength = minimumStandardPipeLength,
            BottomPipeLength = minimumStandardPipeLength,
            RouteBottom = maximumOpenBottom,
            RouteTop = maximumOpenTop
        };
    }

    private PipeLayout CreateRandomStandardLayout()
    {
        float gapSize = Random.Range(minimumGapSize, maximumGapSize);
        float lowestCenter = GetLowestStandardGapCenter(gapSize);
        float highestCenter = GetHighestStandardGapCenter(gapSize);
        float gapCenter = Random.Range(lowestCenter, highestCenter);
        return CreateStandardLayout(gapCenter, gapSize);
    }

    private PipeLayout CreateOriginalFallbackLayout(PipeObstacleType type)
    {
        switch (type)
        {
            case PipeObstacleType.BottomOnly:
                return CreateBottomOnlyLayout(minimumPipeLength);

            case PipeObstacleType.TopOnly:
                return CreateTopOnlyLayout(minimumPipeLength);

            case PipeObstacleType.AsymmetricPair:
                return CreateAsymmetricLayout(minimumPipeLength, minimumPipeLength);

            default:
                return CreateStandardLayout(0f, StandardFallbackGapSize);
        }
    }

    private PipeLayout CreateOverlappingStandardLayout(
        float previousRouteBottom,
        float previousRouteTop)
    {
        float gapSize = Mathf.Clamp(
            StandardFallbackGapSize,
            minimumGapSize,
            maximumGapSize);
        float previousCenter = (previousRouteBottom + previousRouteTop) * 0.5f;
        float gapCenter = Mathf.Clamp(
            previousCenter,
            GetLowestStandardGapCenter(gapSize),
            GetHighestStandardGapCenter(gapSize));
        return CreateStandardLayout(gapCenter, gapSize);
    }

    private PipeLayout CreateBottomOnlyLayout(float pipeLength)
    {
        return new PipeLayout
        {
            Type = PipeObstacleType.BottomOnly,
            HasBottomPipe = true,
            BottomPipeLength = pipeLength,
            RouteBottom = lowerPipeExtent + pipeLength,
            RouteTop = upperPipeExtent
        };
    }

    private PipeLayout CreateTopOnlyLayout(float pipeLength)
    {
        return new PipeLayout
        {
            Type = PipeObstacleType.TopOnly,
            HasTopPipe = true,
            TopPipeLength = pipeLength,
            RouteBottom = groundTopY,
            RouteTop = upperPipeExtent - pipeLength
        };
    }

    private PipeLayout CreateAsymmetricLayout(float topLength, float bottomLength)
    {
        return new PipeLayout
        {
            Type = PipeObstacleType.AsymmetricPair,
            HasTopPipe = true,
            HasBottomPipe = true,
            TopPipeLength = topLength,
            BottomPipeLength = bottomLength,
            RouteBottom = lowerPipeExtent + bottomLength,
            RouteTop = upperPipeExtent - topLength
        };
    }

    private PipeLayout CreateStandardLayout(float gapCenter, float gapSize)
    {
        float gapBottom = gapCenter - gapSize * 0.5f;
        float gapTop = gapCenter + gapSize * 0.5f;
        return new PipeLayout
        {
            Type = PipeObstacleType.StandardPair,
            HasTopPipe = true,
            HasBottomPipe = true,
            TopPipeLength = upperPipeExtent - gapTop,
            BottomPipeLength = gapBottom - lowerPipeExtent,
            RouteBottom = gapBottom,
            RouteTop = gapTop
        };
    }

    private float GetLowestStandardGapCenter(float gapSize)
    {
        return Mathf.Max(
            minimumGapCenter,
            lowerPipeExtent + minimumStandardPipeLength + gapSize * 0.5f);
    }

    private float GetHighestStandardGapCenter(float gapSize)
    {
        return Mathf.Min(
            maximumGapCenter,
            upperPipeExtent - minimumStandardPipeLength - gapSize * 0.5f);
    }
}
