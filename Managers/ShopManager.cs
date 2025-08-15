using Common.Data;
using Services;

namespace Managers{
    public class ShopManager : Singleton<ShopManager> {

        public void Init() {
            NPCManager.Instance.RegisterNpcEvent(NpcDefine.NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop (NpcDefine npc){
            this.ShowShop(npc.Param);
            return true;
        }

        public void ShowShop(int shopId) {
            ShopDefine shop;
            if(DataManager.Instance.Shops.TryGetValue(shopId,out shop)) {

                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                //using UImanager to generate the UIShop
                if (uiShop != null) {
                    uiShop.SetShop(shop);
                }
            }
        }

        //in this statement,we indirectly send a purchase request to server using ItemService
        public bool BuyItem(int shopId,int shopItemId) {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }

    }

}
