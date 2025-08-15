using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.UI {
    //every UI with moneyText should handle to update  moneyText 
    public interface IMoneyUpdate {
        void UpdateMoney();
    }
}
