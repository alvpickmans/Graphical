using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Geometry;
using Graphical.DataStructures;
using Graphical.Extensions;


namespace Graphical.Core
{
    internal class EventChainer
    {
        internal BooleanType booleanType;
        internal Graphical.Graphs.Graph graph = new Graphs.Graph();

        public EventChainer(BooleanType booleanType)
        {
            this.booleanType = booleanType;
        }

        public void Add(SweepEvent swEvent)
        {
            if (IsValidEvent(swEvent))
            {
                graph.AddEdge(swEvent.Edge);
            }
        }

        public List<gPolygon> GetPolygons()
        {
            graph.BuildPolygons();
            return graph.Polygons;
        }

        internal bool IsValidEvent(SweepEvent swEvent)
        {
            bool subjectOut = swEvent.polygonType == PolygonType.Subject && !swEvent.IsInside.Value;
            bool subjectIn = swEvent.polygonType == PolygonType.Subject && swEvent.IsInside.Value;
            bool clipIn = swEvent.polygonType == PolygonType.Clip && swEvent.IsInside.Value;

            switch (booleanType)
            {
                case BooleanType.Union:
                    return !swEvent.IsInside.Value &&
                    swEvent.Label != SweepEventLabel.NoContributing &&
                    swEvent.Label != SweepEventLabel.DifferentTransition;

                case BooleanType.Differenece:
                    return (subjectOut || clipIn) &&
                    swEvent.Label != SweepEventLabel.NoContributing &&
                    swEvent.Label != SweepEventLabel.SameTransition;

                case BooleanType.Intersection:
                    return (subjectIn || clipIn) &&
                    swEvent.Label != SweepEventLabel.NoContributing;
                default:
                    throw new Exception("WARNING! BooleanType is not set up");
            }
        }
    }
}
