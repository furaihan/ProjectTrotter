using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using BarbarianCall.Types;

namespace BarbarianCall.Callouts
{
    class StolenBoatTrailer :CalloutBase
    {
        private Model longfinModel = "h4_prop_h4_p_boat_01a";
        private List<Model> truckModels = new List<Model>()
        {
            "PHANTOM", "PHANTOM2", "PHANTOM3", "HAULER", "HAULER2", "PACKER"
        };
        private Model trailerModel = "TRFLAT";
        private List<Spawnpoint> CalloutLocations = new List<Spawnpoint>()
        {

        };
        string[] evidences = { "collision_ppxega", "collision_888mfmh", "collision_8rpar94", "A paper containing a 6 digit unknown number", "A circuit board with an antenna" };
        List<List<string>> weaponLoadouts = new List<List<string>>()
        {
            new List<string>()
            {
                "WT_SG_ASL",
                "WT_MACHPIST",
                "WT_MACHETE",
                "WT_GNADE"
            },
            new List<string>()
            {
                "WT_MLTRYRFL",
                "WTU_PIST_50",
                "WT_KNUCKLE",
                "WT_GNADE_STK",
            },
            new List<string>()
            {
                "WT_SNIP_RIF",
                "WT_PIST_AP",
                "WT_KNIFE",
                "WT_MOLOTOV",
            },
            new List<string>()
            {
                "WT_SMG",
                "WT_SNSPISTOL",
                "WT_KNIFE",
                "WT_PIPE",
                "collision_7qaosgf", //Hollow Point Rounds
            },
            new List<string>()
            {
                "WT_RIFLE_ASL",
                "WT_PIST",
                "WT_MACHETE",
                "WT_PIPE",
                "collision_39v6ia" //Full Metal Jacket Rounds
            },
        };
    }
}
