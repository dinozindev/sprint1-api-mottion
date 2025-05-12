UBUNTU="Canonical:ubuntu-24_04-lts:server:24.04.202502210"
VM_SIZE="Standard_B2s"
TASK="1"
LOCATION="brazilsouth"

az group create -g rg-sprint1-api-mottion-2tdsb-$LOCATION -l $LOCATION --tags Sprint=$TASK

az network vnet create -n vnet-sprint1-api-mottion-2tdsb-$LOCATION -g rg-sprint1-api-mottion-2tdsb-$LOCATION --tags Sprint=$TASK

az network vnet subnet create -n snet-sprint1-api-mottion-2tdsb-main -g rg-sprint1-api-mottion-2tdsb-$LOCATION --vnet-name vnet-sprint1-api-mottion-2tdsb-$LOCATION --address-prefixes 10.0.0.0/24

az network nsg create -n nsg-sprint1-api-mottion-2tdsb-$LOCATION -g rg-sprint1-api-mottion-2tdsb-$LOCATION --tags Sprint=$TASK

az network nsg rule create -n ssh --nsg-name nsg-sprint1-api-mottion-2tdsb-$LOCATION --priority 1000 --direction Inbound --destination-address-prefixes VirtualNetwork --destination-port-ranges 22 -g rg-sprint1-api-mottion-2tdsb-$LOCATION --protocol Tcp

az vm create -n vm-sprint1-api-mottion-2tdsb-$LOCATION -g rg-sprint1-api-mottion-2tdsb-$LOCATION --image $UBUNTU --size $VM_SIZE --vnet-name  vnet-sprint1-api-mottion-2tdsb-$LOCATION --subnet snet-sprint1-api-mottion-2tdsb-main --nsg nsg-sprint1-api-mottion-2tdsb-$LOCATION --authentication-type password --admin-username azureuser --admin-password Fiap2TDSB2025