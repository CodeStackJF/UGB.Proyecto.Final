using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.Final.Interfaces
{
    public interface IOtpService
    {
        public Task<otp> GetLastByUser(int userId);
        public Task<otp> GetByRequestIdAndOTP(string requestId, string OTP);
        public Task<otp> Insert(otp otp);
        public Task Verify(int id);
    }
}