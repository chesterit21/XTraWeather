using XTramile.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTramile.Infrastructure.Configuration;

namespace XTramile.Infrastructure.IServiceProvider
{
    public interface IXtramileCityService
    {
        Task<IEnumerable<City>> GetListCity(string path,string country);
        StatusFile StatusFileInfo {get;set;} // Actualy better in one service an than This Interface Inhetrit them with this properti, cause share with Country Service..but it is ok lah ya.
    }


}
