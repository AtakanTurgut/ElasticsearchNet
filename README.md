# Used Packages for ElasticsearchNet 
Packages can be installed from the "[.NET CLI](https://learn.microsoft.com/tr-tr/dotnet/core/tools/)".
- [Microsoft.EntityFrameworkCore.SqlServer 6.0.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/6.0.0)
```
    > dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.0
```
- [Microsoft.EntityFrameworkCore.Tools 6.0.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/6.0.0)
```
    > dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.0
```
- [Newtonsoft.Json 13.0.3](https://www.nuget.org/packages/Newtonsoft.Json/13.0.3)
```
    > dotnet add package Newtonsoft.Json --version 13.0.3
```
- + [Elasticsearch.Net 6.0.0](https://www.nuget.org/packages/Elasticsearch.Net/6.0.0)
```
    > dotnet add package Elasticsearch.Net --version 6.0.0
```

## Migrations
Use this commands for the `Migration Operations`:
- Create Migration  
```
    > Add-Migration init
```
- Update Data   
```
    > Update-Database
```

## .yml
```yml
docker.yml

version: '3.8'

services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.7.1
    environment:
      - xpack.security.enabled=false
      - "discovery.type=single-node"
    ports: 
      - 9200:9200
    volumes:
     - elasticsearch-data:/usr/share/elasticsearch/data

  kibana:
    image: docker.elastic.co/kibana/kibana:8.7.1
    ports:
      - 5601:5601
    environment:
      ELASTICSEARCH_HOSTS: http://elasticsearch:9200

volumes:
  elasticsearch-data:
    driver: local
```
```cs
    docker-compose -f docker.yml up -d
```
--------
[Elasticsearch](https://hub.docker.com/_/elasticsearch) |  [Kibana](https://hub.docker.com/_/kibana)  
