using System.Security.Cryptography.X509Certificates;

namespace SmallsOnline.MsGraphClient.Models;

public partial class GraphClient : IGraphClient
{
    /// <summary>
    /// Get a certificate from the local machine.
    /// </summary>
    /// <param name="thumprint">The certificate's thumbprint.</param>
    /// <returns>A certificate.</returns>
    /// <exception cref="Exception"></exception>
    private static X509Certificate2 GetCertificate(string thumprint)
    {
        X509Certificate2 cert;

        using (X509Store certStore = new(StoreLocation.LocalMachine))
        {
            try
            {
                certStore.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certsInStore = certStore.Certificates;
                X509Certificate2Collection foundCerts = certsInStore.Find(X509FindType.FindByThumbprint, thumprint, false);

                if (foundCerts.Count != 1)
                {
                    throw new Exception("Too many certificates were returned with the same thumbprint or no certificates were found.");
                }

                cert = foundCerts[0];
            }
            finally
            {
                certStore.Close();
            }
        }

        return cert;
    }
}