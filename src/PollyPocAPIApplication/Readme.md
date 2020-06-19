# WRONG USAGE OF HTTP CLIENT

    ## ISNTANCE PER REQUEST : http://localhost:57697/api/externalapi/option1
    ## STATIC INSTANCE : http://localhost:57697/api/externalapi/option2

# API ENDPOINT

    - Faulty : http://localhost:57697/api/catalog/2

    ## Reactive Strategies

    	- Retry Polly: http://localhost:57697/api/retry/catalog/2

    	- Wait N Retry Polly: http://localhost:57697/api/waitnretry/catalog/2

    	- CircuitBreaker Polly: http://localhost:57697/api/circuitbreaker/catalog/2

    	- Fallback Polly: http://localhost:57697/api/fallback/catalog/2

    ## Proactive Strategies

    	- Cache Polly:

    	- Timeout Polly: http://localhost:57697/api/timeout/catalog/2

    	- Bulk Head Polly: http://localhost:57697/api/bulkhead/catalog/2

    Policy Wrap

    	- http://localhost:57697/api/policywrap/catalog/2
