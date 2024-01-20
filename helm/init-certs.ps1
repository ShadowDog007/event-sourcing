# Requires step CLI, see https://smallstep.com/docs/step-cli/installation
$linkerdValuesDir = "$PSScriptRoot/values/linkerd"

Write-Host "Generating certificates in $linkerdValuesDir"
step certificate create root.linkerd.cluster.local $linkerdValuesDir/ca.crt $linkerdValuesDir/ca.key `
    --profile root-ca --no-password --insecure

step certificate create identity.linkerd.cluster.local $linkerdValuesDir/issuer.crt $linkerdValuesDir/issuer.key `
    --profile intermediate-ca --not-after 8760h --no-password --insecure `
    --ca $linkerdValuesDir/ca.crt --ca-key $linkerdValuesDir/ca.key
