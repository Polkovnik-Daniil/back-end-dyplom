using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Pattern {
    public class UnitOfWork {
        private AppDbContext context;
        public UnitOfWork(AppDbContext appDbContext) {
            context = appDbContext;
        }
    }
}
