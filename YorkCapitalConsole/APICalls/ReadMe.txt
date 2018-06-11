Json Stucture/Structure
=====================
/*
 TokenMaster - if present, makes sure that subscription is not emited for that Prospect.
*/

{
  "APIProspects" :[
                      { "Base"        : { "BaseUrl" : "https://www.alphavantage.co/query", "Key":"LBMNIZVTOTUQRGRT", "TokenMaster" : "TokenReceiver" } },
                      
                      { "APIProspect" : {
                                          "Name"    : "TokenReceiver",   /*This is assigned as TokenMaster at BASE, and there fore will not emit back to caller.*/
                                          "BaseUrl" : "https://nycsca.proest.com/external_api/v1",
                                          "Uri"     : "login",
                                          "Method"  : "POST",										  
										  "GenericType" : "APICalls.Example.Tokens",
                                        										  
                                          "Parameters" : 
										  {
											  "ParamProperties" : {  "QueryString" : "True", "ContentType" : "application/json" },
											  "ParamValues" :
											  [
												{ "Key" : "partner_key",  "Value" : "UtZ5UUsaj3eW-HWyxV6N" 	},
												{ "Key" : "company_key",  "Value" : "JzxqMBk43yAUBRWYySC4" }												
											  ]
										  },
										  "ContentTypes" : "application/json"										
                                        }
                      },

					  { "APIProspect" : {
                                          "Name"    : "CurrencyExchangeRate",
                                          "BaseUrl" : "",
                                          "Uri"     : "",
                                          "Method"  : "GET",
										  "IncludeKeyFromBase" : "apikey",
										  "GenericType" : "APICalls.Example.RealtimeCurrencyExchange",
                                        										  
										  "Authorization" : 
										  {
										      "Type" : "Bearer", "Token" : "{AnyProspectName.AnyProspectProperty}", "TokenAsHeader": "True" 
										  },
										  
                                          "Headers" : 
                                          [
                                            { "Key" : "CallerKey",       "Value" : "abcCallerInfo"       } ,
                                            { "Key" : "CalculatedValue", "Value" : "{Project.HardValue}" } 
                                          ],
                                          										  
                                          "Parameters" : 
										  {
											  "ParamProperties" : {  "QueryString" : "True", "ContentType" : "application/json" },
											  "ParamValues" :
											  [
												{ "Key" : "function", 		"Value" : "CURRENCY_EXCHANGE_RATE" 			},
												{ "Key" : "from_currency",  "Value" : "{ExchangeCurrency.FromCurrency}" },
												{ "Key" : "to_currency", 	"Value" : "{ExchangeCurrency.ToCurrency}"  	}
											  ]
										  },
										  "ContentTypes" : "application/json",
										  "Repeat" : "2"
                                        }
                      },
                      
                      { "APIProspect" : {
                                         ...
                                        }
                      }
                  ]
}



xml schema
<!--
 TokenMaster - if present, makes sure that subscription is not emited for that Prospect.
-->
<?xml version="1.0" ?> 

<!-- Based on API(s) from : https://www.alphavantage.co/documentation/  Registered here with s...software@gmail.com-->
  
<APIProspects>
  <Base BaseUrl="https://www.alphavantage.co/query"  Key="LBMNIZVTOTUQRGRT" TokenMaster = "TokenReceiver" />

  <APIProspect Name ="TokenReceiver" BaseUrl="https://nycsca.proest.com/external_api/v1" GenericType = "APICalls.Example.Tokens" APIUri="login" Method="POST" Order = "1">
  <!-- This is assigned as TokenMaster at BASE, and there fore will not emit back to caller. -->
		<Parameters>			
			<Parameter Key="partner_key" Value="UtZ5UUsaj3eW-HWyxV6N" />
			<Parameter Key="company_key" Value="JzxqMBk43yAUBRWYySC4" />			
		</Parameters>
  </APIProspect>	

  <APIProspect Name="CurrencyExchangeRate" BaseUrl="" Uri="" Method="GET" IncludeKeyFromBase="apikey" GenericType="APICalls.Example.RealtimeCurrencyExchange" Repeat="2">
    
	<Authorization Type="Bearer/Token/Auth" Token="{AnyNamedProspect.AnyProperty}" TokenAsHeader="True" />

	<Headers>
	  <Header Key="" Value ="" />
	  <Header Key="" Value ="" />
	  ...
	</Headers>

    <Parameters QueryString="True" ContentType="application/json">
      <Parameter Key="function" Value="CURRENCY_EXCHANGE_RATE"/>
      <Parameter Key="from_currency"  Value="{ExchangeCurrency.FromCurrency}"/>
      <Parameter Key="to_currency"  Value="{ExchangeCurrency.ToCurrency}"/>      
    </Parameters>
  
    <ContentTypes Values="application/json" />
  </APIProspect>
  
  <APIProspect ...>
  
</APIProspects>




