# MQAdapter
 Simple .Net Core adapter for IBM MQ

  c:>docker run --env LICENSE=accept --env MQ_QMGR_NAME=QM1 --volume qm1data:/mnt/mqm --publish 1414:1414 --publish 9443:9443 --detach --env MQ_APP_PASSWORD=passw0rd ibmcom/mq:latest

 c:>docker run --env LICENSE=accept --env MQ_QMGR_NAME=QM1 --publish 1414:1414 --publish 9443:9443 --detach ibmcom/mq

 c:>docker ps

 cli$ dspmqver

 cli$ dspmq


 https://developer.ibm.com/components/ibm-mq/tutorials/mq-connect-app-queue-manager-containers/

 https://dotnet-talk.com/communication-with-IBMMQ-in-dotnet-core.html

https://referbruv.com/blog/posts/implementing-a-worker-service-in-aspnet-core



local MQ Series
https://localhost:9443/

user: admin	
pw: passw0rd






