using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.Entities;
using UGB.Proyecto.Final.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace UGB.Proyecto.Final.Repositories
{
    public class OtpRepository(StoreCTX ctx) : IOtpService
    {
        public async Task<otp> GetByRequestIdAndOTP(string requestId, string OTP)
        {
            return (await ctx.otp.Where(x=>x.request_id == requestId && x.otp_code == OTP && !x.verified).FirstOrDefaultAsync())!;
        }

        public async Task<otp> GetLastByUser(int userId)
        {
            return (await ctx.otp.Where(x=>x.user_id == userId).OrderByDescending(x=>x.created_at).FirstOrDefaultAsync())!;
        }

        public async Task<otp> Insert(otp otp)
        {
            ctx.otp.Add(otp);
            await ctx.SaveChangesAsync();
            return otp;
        }

        public async Task Verify(int id)
        {
            var result = await ctx.otp.FindAsync(id);
            if(result == null)
            return;
            result.verified = true;
            await ctx.SaveChangesAsync();
        }
    }
}