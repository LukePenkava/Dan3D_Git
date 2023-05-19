using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    //Each item can have one or multiple tags
    public enum ItemTags {
        Neutral,

        //General Tags
        Resource,
        Food,
        Potion,        
        Recipe,
        Blueprint,

        //Properties
        Consumable,
        Unique,

        //Subtypes
        //Wood,
        //Stone
        //Plant,
        //Gemstone,
        //Ingredient,

        //Other
    }

    public enum ItemName {
        NotDefined,

        //Resources
        Wood,
        Stone,
        Everflour,
        CrunchyCrystal,

        //Food
        Noodles,
        MysticMunchies

        // LavenderSprouts,    
        // GiggleLink,
        // MeadowTail,     
        // CrimsonTie,
        // Gloombrella,
        // Stardew,
        // EffervescentResin,
        // Celandine,
        // Algae,
        // CrystalizedMoonlight,
        // WhiteJade,
        // SpitResidue,
        // AzureYeast,
        // Everflour,   
        // GlimmeringPebble,
        // DustOfNoUse,   
        // MoldyRock,  
        // ElderBerries,
        
        //Food
        //MysteryStew,
        //SparklingBrew,    
        //EverNoodles,
        //BlueberryPie,
        //WitchesButter,
        //HotCocoa,
        //ElderCharm,

        //Potions
        //MysticMortimer,
        //CurseResist,

        //Items
        //ManureAllure,
        //SpatulaofEternity,

        //Recipes
        //RecipeElderCharm
    };
}