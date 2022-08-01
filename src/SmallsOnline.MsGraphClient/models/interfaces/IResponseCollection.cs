namespace SmallsOnline.MsGraphClient.Models;

public interface IResponseCollection<T>
{
    string OdataContext { get; set; }
    string OdataNextLink { get; set; }
    List<T> Value { get; set; }
}