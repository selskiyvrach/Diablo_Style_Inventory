using System;



namespace MNS.Utils
{

    public static class SimpleID 
    {
        private static int _nextId = int.MinValue;

        /* if there are any long-livivng items with iD's their iD's should be reclaimed
        upon reaching maxValue so consider subcribing to this in case this limit (~4 billions) might be surpassed */
        public static event Action OnIDsReset;

        public static int GetNewID()
        {
            var iD = _nextId++;

            if(_nextId == int.MaxValue)
            {
                _nextId = int.MinValue;
                OnIDsReset.Invoke();
            }

            return iD;
        }
    }
    
}
