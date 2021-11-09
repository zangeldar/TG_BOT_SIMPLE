using System;
using System.Threading;
using TdLib;


namespace MyTelegram
{
    public class example
    {
        public async void testAsync(string phoneNumber)
        {
            using (var client = new TdClient())
            {
                try
                {
                    TdApi.Ok ok = await client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
                    {
                        PhoneNumber = phoneNumber
                    });
                    //
                    ok = await client.SetAuthenticationPhoneNumberAsync(phoneNumber);                    
                }
                catch (Exception e)
                {
                    //TdApi.Error error = e;
                    throw;
                }
            }
        }

        public void test(string phoneNumber)
        {
            using (var client = new TdClient())
            {
                try
                {
                    TdApi.Ok ok = client.Execute(new TdApi.SetAuthenticationPhoneNumber
                    {
                        PhoneNumber = phoneNumber
                    });
                    //
                    
                    
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

    }
}
