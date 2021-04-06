using System;

namespace D2Inventory
{

    public class ProjectionEventArgs : EventArgs
    {

        public static readonly ProjectionEventArgs EmptyProjection = new ProjectionEventArgs(Projection.EmptyProjection); 

        public static readonly ProjectionEventArgs SameProjection = new ProjectionEventArgs(Projection.SameProjection); 
        
        public Projection Projection { get; private set; }

        public ProjectionEventArgs(Projection proj)    
            => Projection = proj;
    }
    
}

