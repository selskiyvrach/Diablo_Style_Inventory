using System;


namespace D2Inventory.Control
{

    public class BoolEventArgs : EventArgs
    {
        
        public static readonly BoolEventArgs True = new BoolEventArgs(true);

        public static readonly BoolEventArgs False = new BoolEventArgs(false);

        public bool Value { get; private set; }

        private BoolEventArgs(bool value) { Value = value; }

        public static BoolEventArgs GetArgs(bool value) => value ? True : False;

    }
    
}

