// using System;
// using DataLayer.Tables;
// using NetworkLibrary.NetworkLibrary.Http;
//
// namespace AmoebaGameMatcherServer.Services.Shop.ShopModel.DeleteMeShopSectionFactories
// {
//     public class iscountsSectionFactory
//     {
//         public SectionModel Create()
//         {
//             SectionModel sectionModel = new SectionModel
//             {
//                 NeedFooterPointer = true,
//                 HeaderName = "SPECIAL OFFER"
//             };
//             sectionModel.UiItems = new ProductModel[1][];
//             
//             //первая строка
//             sectionModel.UiItems[0] = new[]
//             {
//                 new ProductModel
//                 {
//                     Id = 2,
//                     TransactionType = TransactionTypeEnum.LootboxOpening,
//                     CurrencyTypeEnum = CostTypeEnum.HardCurrency,
//                     ImagePreviewPath = "BigLootbox",
//                     Name = "MEGA BOX",
//                     CostString = 139.ToString(),
//                     Cost = 139,
//                     ShopItemSize = ProductSizeEnum.Big,
//                     DiscountPrice = new DiscountPrice
//                     {
//                         CostBeforeDiscount = 320
//                     },
//                     UtcDeadline = DateTime.UtcNow+TimeSpan.FromHours(4)
//                 }
//             };
//
//             return sectionModel;
//         }
//     }
// }