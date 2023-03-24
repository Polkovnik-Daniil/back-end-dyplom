using DBManager.Pattern.Repositories;

namespace DBManager.Pattern {
    public class UnitOfWork {
        private AppDbContext ApplicationDbСontext;
        private UserRepository _userRepository;
        private RoleRepository _roleRepository;
        private UserLoginRepository _userLoginRepository;
        public UnitOfWork(AppDbContext appDbContext) {
            ApplicationDbСontext = appDbContext;
        }
        public UserRepository Users {
            get {
                if(_userRepository == null) {
                    _userRepository = new UserRepository(ApplicationDbСontext);
                }
                return _userRepository;
            }
        }
        public RoleRepository Roles {
            get {
                if (_roleRepository == null) {
                    _roleRepository = new RoleRepository(ApplicationDbСontext);
                }
                return _roleRepository;
            }
        }
        public UserLoginRepository UserLogins {
            get {
                if (_userLoginRepository == null) {
                    _userLoginRepository = new UserLoginRepository(ApplicationDbСontext);
                }
                return _userLoginRepository;
            }
        }
    }
}
