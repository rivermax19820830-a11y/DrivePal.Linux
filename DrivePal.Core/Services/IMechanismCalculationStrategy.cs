using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrivePal.Core.Dtos;
using DrivePal.Core.Models;
using DrivePal.Core.POCO;

namespace DrivePal.Core.Services
{
    public interface IMechanismCalculationStrategy
    {
        //double CalculateTotalNetPower(IMechanism mechanism);

        //double CalculateSingleNetPower(IMechanism mechanism, double totalNetPower);

        //double CalculateScaledPower(IMechanism mechanism, double singleNetPower);

        //double CalculateLoadCurrent(IMechanism mechanism, double scaledPower);

        //double CalculateInverterRatedCurrent(IMechanism mechanism, double loadCurrent);

        //double CalculateInverterOverloadCurrent(IMechanism mechanism, double loadCurrent, double overloadMultiple = 1.5);

        //Task<List<RecommendedInverterDto>> SelectSuitableInvertersAsync(IMechanism mechanism, NetPowerFilterCriteria criteria);




        void Calculate(IMechanism mechanism);


    }
}
