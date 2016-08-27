using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PokemonGoGetLog.Models
{
    //変更したらPM> Update-Database
    public class PokemonGetData
    {
        public long PokemonGetDataId { get; set; }

        [Display(Name = "ポケモン")]
        public string PokemonName { get; set; }
        public string PokemonImageName { get; set; }
        public IEnumerable<PokemonData> Pokemons { get; set; }

        [Display(Name = "ゲットした場所")]
        [Required(ErrorMessage = "ゲットした場所が入力されていません。")]
        public string Position { get; set; }
        public double MapX { get; set; }
        public double MapY { get; set; }

        [Display(Name = "CP")]
        public int? Cp { get; set; }

        [Display(Name = "ユーザー名")]
        public string User { get; set; }

        [Display(Name = "画像リンク")]
        public string ImageUrl { get; set; }

        //http://benjii.me/2014/03/track-created-and-modified-fields-automatically-with-entity-framework-code-first/
        [Display(Name = "作成日時")]
        public DateTime CreateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }
    }
}
