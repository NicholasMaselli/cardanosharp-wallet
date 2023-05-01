using Xunit;
using System;
using System.Runtime.InteropServices;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CsBindgen;

namespace CardanoSharp.Wallet.Test
{
    public class UPLCTests
    {
        public UPLCTests() { }

        [Fact]
        public void NativeParameterizedContractTest()
        {
            string expectedScriptCBORHex = "5902920100003323232323232323232323232323232232222533300c323232323232323232323232323232533302030230021323232533301e3370e9000000899299980f99b87003480084cdc780200b8a50301c32533301f3370e9000180f00088008a99810a4812a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e001632323300100d23375e6603a603e002900000c18008009112999813001099ba5480092f5c026464a6660466006004266e952000330290024bd700999802802800801981500198140010a99980f19b8700233702900024004266e3c00c058528180e0099bad3020002375c603c0022a6603a9201334c6973742f5475706c652f436f6e73747220636f6e7461696e73206d6f7265206974656d73207468616e206578706563746564001630210013200132323232533301d3370e90010008a5eb7bdb1804c8c8004dd59812800980d801180d8009980080180518008009112999810801099ba5480092f5c0264646464a66604066e3c0140044cdd2a40006604c6e980092f5c0266600e00e00600a6eb8c08800cdd59811001181280198118011bab301f001301f001301e001301d001301c00237586034002602000a6eb8c060004c0394ccc040cdc3a4000601e00220022a660249212a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e0016301600130160023014001300a001149858dd7000980080091129998068010a4c2660126002601e00466600600660200040026600200290001111199980399b8700100300e233330050053370000890011808000801001118039baa001230053754002ae695cdab9c5573aaae7955cfaba05742ae89300104439873450001";
            
            // Parameters            
            // This is the Aiken CBOr from the "gift card" example
            string compiledAikenCBOR = "590288010000323232323232323232323232323232232222533300c323232323232323232323232323232533302030230021323232533301e3370e9000000899299980f99b87003480084cdc780200b8a50301c32533301f3370e9000180f00088008a99810a492a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e001632323300100d23375e6603a603e002900000c18008009112999813001099ba5480092f5c026464a6660466006004266e952000330290024bd700999802802800801981500198140010a99980f19b8700233702900024004266e3c00c058528180e0099bad3020002375c603c0022a6603a9201334c6973742f5475706c652f436f6e73747220636f6e7461696e73206d6f7265206974656d73207468616e206578706563746564001630210013200132323232533301d3370e90010008a5eb7bdb1804c8c8004dd59812800980d801180d8009980080180518008009112999810801099ba5480092f5c0264646464a66604066e3c0140044cdd2a40006604c6e980092f5c0266600e00e00600a6eb8c08800cdd59811001181280198118011bab301f001301f001301e001301d001301c00237586034002602000a6eb8c060004c0394ccc040cdc3a4000601e00220022a660249212a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e0016301600130160023014001300a001149858dd7000980080091129998068010a4c2660126002601e00466600600660200040026600200290001111199980399b8700100300e233330050053370000890011808000801001118039baa001230053754002ae695cdab9c5573aaae7955cfaba05742ae89";

            // Plutus TokenName data
            byte[] tokenName = "987345".HexToByteArray();
            PlutusDataBytes plutusDataBytes = new PlutusDataBytes() { Value = tokenName };
            IPlutusData[] plutusDataParameters = new IPlutusData[] { plutusDataBytes };
            PlutusDataArray plutusDataArray = new PlutusDataArray() { Value = plutusDataParameters };

            // Get Untyped Plutus Core (UPLC) Function Parameters
            byte[] paramsArray = plutusDataArray.Serialize();
            byte[] plutusScriptArray = compiledAikenCBOR.HexToByteArray();

            nuint paramsLength = (nuint)paramsArray.Length;
            nuint plutusScriptLength = (nuint)plutusScriptArray.Length;

            PlutusScriptResult result;
            string actualScriptCBORHex = "";
            unsafe {
                fixed (byte* paramsPtr = &paramsArray[0])
                fixed (byte* plutusScriptPtr = &plutusScriptArray[0])
                
                result = UPLCNativeMethods.apply_params_to_plutus_script(paramsPtr, plutusScriptPtr, paramsLength, plutusScriptLength);
                
                byte[] byteArray = new byte[result.length];
                Marshal.Copy((IntPtr)result.value, byteArray, 0, (int)result.length);

                actualScriptCBORHex = byteArray.ToStringHex();
            };

            Assert.Equal(expectedScriptCBORHex, actualScriptCBORHex);
        }

        [Fact]
        public void ParameterizedContractTest()
        {
            string expectedScriptCBORHex = "5902920100003323232323232323232323232323232232222533300c323232323232323232323232323232533302030230021323232533301e3370e9000000899299980f99b87003480084cdc780200b8a50301c32533301f3370e9000180f00088008a99810a4812a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e001632323300100d23375e6603a603e002900000c18008009112999813001099ba5480092f5c026464a6660466006004266e952000330290024bd700999802802800801981500198140010a99980f19b8700233702900024004266e3c00c058528180e0099bad3020002375c603c0022a6603a9201334c6973742f5475706c652f436f6e73747220636f6e7461696e73206d6f7265206974656d73207468616e206578706563746564001630210013200132323232533301d3370e90010008a5eb7bdb1804c8c8004dd59812800980d801180d8009980080180518008009112999810801099ba5480092f5c0264646464a66604066e3c0140044cdd2a40006604c6e980092f5c0266600e00e00600a6eb8c08800cdd59811001181280198118011bab301f001301f001301e001301d001301c00237586034002602000a6eb8c060004c0394ccc040cdc3a4000601e00220022a660249212a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e0016301600130160023014001300a001149858dd7000980080091129998068010a4c2660126002601e00466600600660200040026600200290001111199980399b8700100300e233330050053370000890011808000801001118039baa001230053754002ae695cdab9c5573aaae7955cfaba05742ae89300104439873450001";
            
            // Parameters            
            // This is the Aiken CBOr from the "gift card" example
            string compiledAikenCBOR = "590288010000323232323232323232323232323232232222533300c323232323232323232323232323232533302030230021323232533301e3370e9000000899299980f99b87003480084cdc780200b8a50301c32533301f3370e9000180f00088008a99810a492a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e001632323300100d23375e6603a603e002900000c18008009112999813001099ba5480092f5c026464a6660466006004266e952000330290024bd700999802802800801981500198140010a99980f19b8700233702900024004266e3c00c058528180e0099bad3020002375c603c0022a6603a9201334c6973742f5475706c652f436f6e73747220636f6e7461696e73206d6f7265206974656d73207468616e206578706563746564001630210013200132323232533301d3370e90010008a5eb7bdb1804c8c8004dd59812800980d801180d8009980080180518008009112999810801099ba5480092f5c0264646464a66604066e3c0140044cdd2a40006604c6e980092f5c0266600e00e00600a6eb8c08800cdd59811001181280198118011bab301f001301f001301e001301d001301c00237586034002602000a6eb8c060004c0394ccc040cdc3a4000601e00220022a660249212a4578706563746564206f6e20696e636f727265637420636f6e7374727563746f722076617269616e742e0016301600130160023014001300a001149858dd7000980080091129998068010a4c2660126002601e00466600600660200040026600200290001111199980399b8700100300e233330050053370000890011808000801001118039baa001230053754002ae695cdab9c5573aaae7955cfaba05742ae89";

            // Plutus TokenName data
            byte[] tokenName = "987345".HexToByteArray();
            PlutusDataBytes plutusDataBytes = new PlutusDataBytes() { Value = tokenName };
            IPlutusData[] plutusDataParameters = new IPlutusData[] { plutusDataBytes };
            PlutusDataArray plutusDataArray = new PlutusDataArray() { Value = plutusDataParameters };

            string actualScriptCBORHex = UPLCMethods.ApplyParamsToPlutusScript(plutusDataArray, compiledAikenCBOR);
            Assert.Equal(expectedScriptCBORHex, actualScriptCBORHex);
        }
    }
}