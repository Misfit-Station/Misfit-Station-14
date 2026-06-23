entity-effect-guidebook-modify-power-amount =
    { $chance ->
        [1] { $deltasign ->
                [1] Increases
                *[-1] Reduces
            }
        *[other] { $deltasign ->
                    [1] increase
                    *[-1] reduce
                 }
    } charge
