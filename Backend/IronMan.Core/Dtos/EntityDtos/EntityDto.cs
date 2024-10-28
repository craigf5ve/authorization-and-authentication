using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.Core.Dtos.EntityDtos
{
    public class EntityDto<T>
    {
        public T Id { get; set; }
    }
}
