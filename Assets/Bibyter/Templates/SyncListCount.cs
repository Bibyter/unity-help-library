using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Bibyter.Templates
{
    static class SyncListCount
    {
        static void Main()
        {
            var syncList = new List<int>();
            var targetCount = 10;

            for (int i = syncList.Count; i < targetCount; i++)
            {
                syncList.Add(i);
            }

            for (int i = syncList.Count - 1; i >= targetCount; i--)
            {
                syncList.RemoveAt(i);
            }
        }
    }
}
