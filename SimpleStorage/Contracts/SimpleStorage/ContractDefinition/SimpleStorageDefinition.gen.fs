namespace Contracts.SimpleStorage.ContractDefinition

open System
open System.Threading.Tasks
open System.Collections.Generic
open System.Numerics
open Nethereum.Hex.HexTypes
open Nethereum.ABI.FunctionEncoding.Attributes
open Nethereum.RPC.Eth.DTOs
open Nethereum.Contracts.CQS
open Nethereum.Contracts
open System.Threading

    
    
    type SimpleStorageDeployment(byteCode: string) =
        inherit ContractDeploymentMessage(byteCode)
        
        static let BYTECODE = "6080806040523460135760c2908160188239f35b5f80fdfe60808060405260043610156011575f80fd5b5f3560e01c90816360fe47b114604d5750636d4ce63c14602f575f80fd5b346049575f36600319011260495760205f54604051908152f35b5f80fd5b346049576020366003190112604957600435805f5581527fe2b64d6eb95e02b6354c9dad0f0b75f05c8659bc5f2a0f91bc457fc27c55c68860203392a200fea26469706673582212207374cead7bbaaf900d27524ee435c8212a7d235b0f71667aef7f83fc50bd64ba64736f6c634300081d0033"
        
        new() = SimpleStorageDeployment(BYTECODE)
        

        
    
    [<Function("get", "uint256")>]
    type GetFunction() = 
        inherit FunctionMessage()
    

        
    
    [<Function("set")>]
    type SetFunction() = 
        inherit FunctionMessage()
    
            [<Parameter("uint256", "x", 1)>]
            member val public X = Unchecked.defaultof<BigInteger> with get, set
        
    
    [<Event("ValueChanged")>]
    type ValueChangedEventDTO() =
        inherit EventDTO()
            [<Parameter("address", "sender", 1, true )>]
            member val Sender = Unchecked.defaultof<string> with get, set
            [<Parameter("uint256", "newValue", 2, false )>]
            member val NewValue = Unchecked.defaultof<BigInteger> with get, set
        
    
    [<FunctionOutput>]
    type GetOutputDTO() =
        inherit FunctionOutputDTO() 
            [<Parameter("uint256", "", 1)>]
            member val public ReturnValue1 = Unchecked.defaultof<BigInteger> with get, set
        
    


