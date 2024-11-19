for %%I in (*.fx) do (
    fxc /T fx_2_0 %%I /Fo %%~nI.fxb
)
