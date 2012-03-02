using Rhino.Mocks;

namespace WebApiContribTests
{
    public class TestBase
    {
        protected T S<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }
    }
}