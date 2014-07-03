<?php

//ini_set('display_errors','On');
//error_reporting(E_ALL);

$wsdl = "http://mydsmoserver/dsmo/lb.asmx?wsdl";
	
$username = "username";
$password = "password";
$docAlias = "PostcardHelp";

$xml = <<<XML
<?xml version="1.0" encoding="utf-8" ?>
<COMMAND>
  <StaticFields>
    <FandV FieldName="Front" FieldValue="[[Firstname]]" FieldType="0" />
    <FandV FieldName="Text" FieldValue="'Xerum fugitint. Dam eatemperem. Ra sediorit porepud ipicienimus eiciati uscillaccust ...'" FieldType="0" />
    <FandV FieldName="AddressLine1" FieldValue="[[Salutation]]" FieldType="0" />
    <FandV FieldName="AddressLine2" FieldValue="[[Firstname]] + ' ' + [[Name]]" FieldType="0" />
    <FandV FieldName="AddressLine3" FieldValue="[[Street]]" FieldType="0" />
    <FandV FieldName="AddressLine4" FieldValue="[[Postcode]] + ' ' + [[City]]" FieldType="0" />
    <FandV FieldName="AddressLine5" FieldValue="[[Country]]" FieldType="0" />
  </StaticFields>
  <TableRows>
    <Fields>
      <FandV FieldName="Salutation" FieldValue="Mr." FieldType="0" />
      <FandV FieldName="Firstname" FieldValue="Tobias" FieldType="0" />
      <FandV FieldName="Name" FieldValue="Knapp" FieldType="0" />
      <FandV FieldName="Street" FieldValue="Alt-Moabit 60" FieldType="0" />
      <FandV FieldName="Postcode" FieldValue="10555" FieldType="0" />
      <FandV FieldName="City" FieldValue="Berlin" FieldType="0" />
      <FandV FieldName="Country" FieldValue="Germany" FieldType="0" />
    </Fields>
    <Fields>
      <FandV FieldName="Salutation" FieldValue="Mr." FieldType="0" />
      <FandV FieldName="Firstname" FieldValue="Oliver" FieldType="0" />
      <FandV FieldName="Name" FieldValue="Dehne" FieldType="0" />
      <FandV FieldName="Street" FieldValue="Alt-Moabit 60" FieldType="0" />
      <FandV FieldName="Postcode" FieldValue="10555" FieldType="0" />
      <FandV FieldName="City" FieldValue="Berlin" FieldType="0" />
      <FandV FieldName="Country" FieldValue="Germany" FieldType="0" />
    </Fields>
  </TableRows>
</COMMAND>
XML;

try
{

	$client = new SoapClient($wsdl, array('soap_version' => SOAP_1_1, "trace" => 1 ));

	
	$sId = $client->Authenticate(array('UserName' => $username,
											'Password' =>$password,
											'Language' => 'EN'))->AuthenticateResult;
	

	if(strtoupper(substr($sId,0,3)) == 'ERR') 
	{
		echo "Authenticate: " . $sId; 
		exit;
	}
	
	echo "Session ID: " . $sId . "</br>";
	
	$orderID = $client->PlaceWorkflowOrder(array('SessionID' => $sId, 
												'DocAlias' => $docAlias,
												'MsgComXML' => $xml,
												'ExtOrderNo' => '',
												'ExtOrderItemID' => ''))->PlaceWorkflowOrderResult;
												
	if(strtoupper(substr($orderID,0,3)) == 'ERR')
	{
		echo $orderID; 
		exit;
	}
	
	echo "Order ID: " . $orderID . "</br>";
	
	$state = $client->Workflow_GetOrderState(array('SessionID' => $sId, 
											 'OrderID' => $orderID))->Workflow_GetOrderStateResult;
	
	if(strtoupper(substr($state,0,3)) == 'ERR')
	{
		echo $state;
		exit;
	}
	
	echo "Current State: " . $state ."</br>";
	
	
}
catch(Exception $ex)
{
	echo $ex->getMessage();
}

?>