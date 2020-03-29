


using EnTier.Context;

namespace EnTier.Controllers{

    public class ControllerConfigurationBuilder{


        private ControllerConfigurations _configuration;


        public ControllerConfigurationBuilder()
        {
            _configuration = new ControllerConfigurations();
        }

        public ControllerConfigurationBuilder GetAll(bool value)
        {
            _configuration.ImplementsGetAll = value;
            return this;
        }

        public ControllerConfigurationBuilder GetById(bool value)
        {
            _configuration.ImplementsGetById = value;
            return this;
        }

        
        public ControllerConfigurationBuilder DeleteById(bool value)
        {
            _configuration.ImplementsDeleteById = value;
            return this;
        }

        public ControllerConfigurationBuilder DeleteByEntity(bool value)
        {
            _configuration.ImplementsDeleteByEntity = value;
            return this;
        }

        public ControllerConfigurationBuilder Update(bool value)
        {
            _configuration.ImplementsUpdate = value;
            return this;
        }

        public ControllerConfigurationBuilder CreateNew(bool value)
        {
            _configuration.ImplementsCreateNew = value;
            return this;
        }


        public ControllerConfigurationBuilder ImplementAll()
        {
            return ImplementAll(true);
        }

        public ControllerConfigurationBuilder ImplementNone()
        {
            return ImplementAll(false);
        }

        private ControllerConfigurationBuilder ImplementAll(bool value)
        {
            _configuration.ImplementsCreateNew = value;
            _configuration.ImplementsDeleteById = value;
            _configuration.ImplementsGetAll = value;
            _configuration.ImplementsGetById = value;
            _configuration.ImplementsUpdate = value;
            _configuration.ImplementsDeleteByEntity = value;
            return this;
        }

        public ControllerConfigurationBuilder ReadonlyImplementation()
        {
            _configuration.ImplementsCreateNew = false;
            _configuration.ImplementsDeleteById = false;
            _configuration.ImplementsGetAll = true;
            _configuration.ImplementsGetById = true;
            _configuration.ImplementsUpdate = false;
            _configuration.ImplementsDeleteByEntity = false;
            return this;
        }

        public ControllerConfigurations Build(){
            return _configuration;
        }

    }

}