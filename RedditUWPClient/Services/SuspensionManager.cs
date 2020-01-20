using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RedditUWPClient.Helpers.Responses;

namespace RedditUWPClient.Services
{

    public class SuspensionState
    {
        public List<Data.Child> Entries { get; set; }
        public Data.Child SelectedEntry { get; set; }
    }

    static internal class SuspensionManager
    {
        private const string SuspensionStateFileNameWithExt = "SuspensionState.txt";

        static internal List<Data.Child> PointerTo_ListOfEntries;
        static internal Data.Child PointerTo_SelectedEntry;

        static internal async Task LoadState()
        {

            //Try catch inside of the Method
            SingleParam<SuspensionState> res = await new Persistance().LoadJsonAsync<SuspensionState>(SuspensionStateFileNameWithExt);
            if (res.Success == true)
            {
                PointerTo_ListOfEntries = res.value.Entries;
                PointerTo_SelectedEntry = res.value.SelectedEntry;
            }
           
        }

        static internal async Task SaveState()
        {

            SuspensionState state = new SuspensionState()
            {
                Entries = PointerTo_ListOfEntries,
                SelectedEntry = PointerTo_SelectedEntry
            };

            //Try catch inside of the Method
            await new Persistance().SaveJsonAsync<SuspensionState>(SuspensionStateFileNameWithExt,state);
          
        }
    }
}
