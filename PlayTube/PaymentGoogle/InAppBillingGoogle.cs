using System.Collections.Generic;

namespace PlayTube.PaymentGoogle
{
    public static class InAppBillingGoogle
    {
        public static readonly string ProductId = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAh5gMjWS8vBRk/QQgW+OLOsJdxg77CSP2aZGsOYFdFPik2LvvXLZC/cxogxsQH3tveF/4ljqMDOV3He6H0UX67q3ukJCgs3v4wmjBu5k+8t3Vrqufk+NZl8vqw3pRFLq7OqHTnIpuQS0pdCp9Af4nGiFo/XbZcaxgh3aVA+gFRtyXtgPVoz5lqFaN50i7JqvhVFBJ4B3Naf4++smbQcNMqjrSPVIrSvrNIv0BOWMP4w6bqluJF/HTl8UMJwd95X4boTkudonhthbcjpYf4lxgIAnoBNhbTfc5a6u57IBuH7I9tnKPcqK/pobMbWHzDqt2ZHcOu20RpiczNPvarvv+twIDAQAB";
        public static readonly List<string> ListProductSku = new List<string> // ID Product
        {
            "membership",
            "rentvideo"
        };
    }
}