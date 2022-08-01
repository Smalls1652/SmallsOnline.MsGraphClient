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

        // Initialize an instance of the local machine's certificate store.
        using (X509Store certStore = new(StoreLocation.LocalMachine))
        {
            try
            {
                // Open the certificate store as read-only.
                certStore.Open(OpenFlags.ReadOnly);

                // Get the certificates in the certificate store.
                X509Certificate2Collection certsInStore = certStore.Certificates;

                // Start a search for the certificate based off the thumprint and for valid certificates.
                X509Certificate2Collection foundCerts = certsInStore.Find(X509FindType.FindByThumbprint, thumprint, false);

                // Thow an error if the number of certificates found is 0 or higher than 1.
                // Only one certificate should be found.
                if (foundCerts.Count != 1)
                {
                    throw new Exception("Too many certificates were returned with the same thumbprint or no certificates were found.");
                }

                // Set 'cert' to the first certificate in the array.
                cert = foundCerts[0];
            }
            finally
            {
                // Whether an error occurred or not,
                // ensure that the certificate store instance is closed.
                certStore.Close();
            }
        }

        // Return the certificate.
        return cert;
    }
}