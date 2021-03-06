//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlcantaraClient.Entities
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    public partial class Basket
    {
        public int ID { get; set; }
        public int IdOrder { get; set; }
        public int IdFurniture { get; set; }
        public int Quantity { get; set; }
    
        public virtual Order Order { get; set; }

        public Furniture Furniture
        {
            get
            {
                Furniture toReturn = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Furnitures.FirstOrDefault(f => f.ID == IdFurniture);

                return toReturn;
            }
        }
    }
}
