/*
Open your project. On each build all *.tt templates will be reprocessed

<!-- This line could already present in file. If it is so just skip it  -->
<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
<!-- process *.tt templates on each build  -->
<PropertyGroup>
    <TransformOnBuild>true</TransformOnBuild>
</PropertyGroup>
<Import Project="$(MSBuildExtensionsPath)\Microsoft\VisualStudio\TextTemplating\v10.0\Microsoft.TextTemplating.targets" />
*/
 
