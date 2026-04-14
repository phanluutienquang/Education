# Authentication & Authorization Architecture Design

## 📊 Complete Flow Diagrams

> **💡 Tip:** Click vào tiêu đề diagram để mở Mermaid Live Editor và xem interactive version!

---

## 1. TOTAL AUTHENTICATION FLOW

[![Authentication Flow](https://mermaid.ink/img/pako:eNrqVspKzE0tLk5MT1WyUoKS8flFJUBqQmlqcXG6kmN-ak6pnZJSfmqxkpJ7fkFxalGJkmNyalFyTqldekrRgCwA2ykjRQ)](https://mermaid.live/edit#pako:eNrqVspKzE0tLk5MT1WyUoKS8flFJUBqQmlqcXG6kmN-ak6pnZJSfmqxkpJ7fkFxalGJkmNyalFyTqldekrRgCwA2ykjRQ)

**🔗 Interactive Link:** https://mermaid.live/edit#pako:eNrqVspKzE0tLk5MT1WyUoKS8flFJUBqQmlqcXG6kmN-ak6pnZJSfmqxkpJ7fkFxalGJkmNyalFyTqldekrRgCwA2ykjRQ

```mermaid
graph TB
    Start([🔐 Client Request]) --> CheckHeaders{📋 Headers Present?}
    
    CheckHeaders -->|❌ No X-Client-Source| Error1[<b>❌ 401<br/>Missing Client Source</b>]
    CheckHeaders -->|❌ No Token| Error2[<b>❌ 401<br/>Missing Token</b>]
    
    CheckHeaders -->|✅ Both Present| RateLimit{⚡ Rate Limit OK?}
    
    RateLimit -->|❌ Exceeded| Error3[<b>❌ 429<br/>Too Many Requests</b>]
    RateLimit -->|✅ OK| CacheCheck{💾 Cache Hit?}
    
    CacheCheck -->|✅ Yes - Hit| ValidateToken[🔍 Validate JWT Token]
    CacheCheck -->|❌ No - Miss| DBLookup[🗄️ Query MongoDB]
    
    DBLookup --> ClientFound{👤 Client Exists?}
    ClientFound -->|❌ No| Error4[<b>❌ 401<br/>Invalid Client</b>]
    ClientFound -->|✅ Yes| CacheStore[💾 Store in Cache]
    
    CacheStore --> ValidateToken
    
    ValidateToken --> TokenValid{✅ Token Valid?}
    TokenValid -->|❌ Expired| CheckRefresh{🔄 Has Refresh Token?}
    TokenValid -->|❌ Invalid Signature| Error5[<b>❌ 401<br/>Invalid Token</b>]
    TokenValid -->|✅ Valid| CheckClient{👤 Client Active?}
    
    CheckRefresh -->|❌ No| Error6[<b>❌ 401<br/>Token Expired</b>]
    CheckRefresh -->|✅ Yes| RefreshFlow[🔄 Generate New Token Pair]
    
    RefreshFlow --> NewTokens[🎫 Return New Access + Refresh Token]
    
    CheckClient -->|❌ Disabled| Error7[<b>❌ 403<br/>Client Disabled</b>]
    CheckClient -->|❌ IP Blocked| Error8[<b>❌ 403<br/>IP Not Allowed</b>]
    CheckClient -->|✅ Active & Valid| AddClaims[➕ Add Claims: ID, Scopes, Roles]
    
    AddClaims --> Success[<b>✅ 200 OK<br/>Process Request</b>]
    
    Error1 --> End([🏁 End])
    Error2 --> End
    Error3 --> End
    Error4 --> End
    Error5 --> End
    Error6 --> End
    Error7 --> End
    Error8 --> End
    Success --> End
    NewTokens --> End
```

---

## 2. TOKEN REFRESH SEQUENCE

```mermaid
sequenceDiagram
    autonumber
    participant C as <b>📱 Client App</b>
    participant GW as <b>🚪 API Gateway</b>
    participant TS as <b>🎫 Token Service</b>
    participant DB as <b>🗄️ MongoDB</b>
    participant RC as <b>⚡ Redis Cache</b>
    
    Note over C,RC: <b>Scenario: Access Token Expired</b>
    
    C->>GW: <b>① Request</b><br/>+ Expired Token
    GW->>TS: <b>② Validate Token</b>
    TS->>TS: Check Expiration
    TS-->>GW: <b>③ Token Expired</b>
    GW-->>C: <b>④ 401 Unauthorized</b><br/><i>Include: Refresh-Token Header</i>
    
    Note over C,RC: <b>Refresh Token Flow</b>
    
    C->>GW: <b>⑤ POST /auth/refresh</b><br/>{refreshToken}
    GW->>TS: <b>⑥ Refresh Token</b>
    
    TS->>RC: <b>⑦ Check if Revoked?</b>
    RC-->>TS: <b>⑧ Not Revoked</b>
    
    TS->>DB: <b>⑨ Get Client Info</b>
    DB-->>TS: <b>⑩ Client Data</b>
    
    TS->>TS: <b>⑪ Validate Refresh Token</b>
    TS->>TS: <b>⑫ Generate New Access Token</b>
    TS->>TS: <b>⑬ Generate New Refresh Token</b>
    
    TS->>RC: <b>⑭ Revoke Old Refresh Token</b>
    TS->>RC: <b>⑮ Store New Refresh Token</b>
    
    TS-->>GW: <b>⑯ New Token Pair</b>
    GW-->>C: <b>⑰ 200 OK</b><br/>{accessToken, refreshToken}
    
    Note over C,RC: <b>Retry with New Token</b>
    
    C->>GW: <b>⑱ Retry Original Request</b><br/>+ New Token
    GW->>TS: <b>⑲ Validate New Token</b>
    TS-->>GW: <b>⑳ Valid ✅</b>
    GW->>C: <b>㉑ 200 OK - Response</b>
```

---

## 3. SYSTEM ARCHITECTURE OVERVIEW

```mermaid
graph TB
    subgraph "🌐 External Clients"
        Client[<b>📱 Mobile/Web App</b>]
        Admin[<b>👨‍💼 Admin Portal</b>]
        ThirdParty[<b>🔗 Third-party API</b>]
    end
    
    subgraph "🚪 API Gateway Layer"
        LB[<b>⚖️ Load Balancer</b>]
        WAF[<b>🛡️ Web Application Firewall</b>]
        GW[<b>🚪 API Gateway / Middleware</b>]
    end
    
    subgraph "🔐 Authentication Services"
        AuthHandler[<b>👮 Auth Handler</b>]
        TokenSvc[<b>🎫 Token Service</b>]
        CacheSvc[<b>💾 Cache Service</b>]
        RateLimit[<b>⚡ Rate Limiter</b>]
        HealthCheck[<b>🏥 Health Monitor</b>]
    end
    
    subgraph "🗄️ Data Layer"
        MongoDB[(<b>📦 MongoDB</b><br/>Client Sources)]
        Redis[(<b>⚡ Redis</b><br/>Cache/Tokens/RateLimit)]
    end
    
    subgraph "💼 Business Services"
        OrderService[<b>📦 Order Service</b>]
        ProductService[<b>🏷️ Product Service</b>]
        ReportService[<b>📊 Report Service</b>]
    end
    
    Client --> LB
    Admin --> LB
    ThirdParty --> LB
    
    LB --> WAF
    WAF --> GW
    
    GW --> RateLimit
    GW --> AuthHandler
    
    AuthHandler --> TokenSvc
    AuthHandler --> CacheSvc
    
    TokenSvc --> MongoDB
    TokenSvc --> Redis
    
    CacheSvc --> Redis
    CacheSvc --> MongoDB
    
    RateLimit --> Redis
    
    AuthHandler --> OrderService
    AuthHandler --> ProductService
    AuthHandler --> ReportService
    
    HealthCheck --> MongoDB
    HealthCheck --> Redis
    HealthCheck --> TokenSvc
```

---

## 4. MIDDLEWARE PIPELINE

```mermaid
graph LR
    Request([📨 HTTP Request]) --> EH[<b>1️⃣ Exception Handler</b><br/>Global error handling]
    
    EH --> CORS[<b>2️⃣ CORS Policy</b><br/>Cross-origin check]
    
    CORS --> RL[<b>3️⃣ Rate Limiter</b><br/>Request throttling]
    
    RL --> AuthN[<b>4️⃣ Authentication</b><br/>Validate token & client]
    
    AuthN --> AuthZ[<b>5️⃣ Authorization</b><br/>Check permissions/scopes]
    
    AuthZ --> Controller[<b>6️⃣ Controller Action</b><br/>Business logic]
    
    Controller --> Response([📤 HTTP Response])
    
    style EH fill:#ffcccc
    style CORS fill:#ffffcc
    style RL fill:#ccffcc
    style AuthN fill:#ccccff
    style AuthZ fill:#ffccff
    style Controller fill:#ccffff
```

---

## 5. RATE LIMITING ALGORITHM

```mermaid
graph TB
    Start([📨 New Request]) --> GetClientId[📋 Extract Client ID]
    
    GetClientId --> GetLimit[⚙️ Get Rate Limit<br/>from config]
    
    GetLimit --> GetCurrentCount[📊 Get Current Count<br/>from Redis]
    
    GetCurrentCount --> CheckLimit{Current < Limit?}
    
    CheckLimit -->|❌ No - Exceeded| Return429[<b>❌ 429 Too Many Requests</b><br/>Add Retry-After header]
    
    CheckLimit -->|✅ Yes - OK| Increment[➕ Increment Counter<br/>in Redis]
    
    Increment --> SetExpiry[⏰ Set TTL = 60s<br/>if first request]
    
    SetExpiry --> Allow[<b>✅ 200 OK</b><br/>Pass to next middleware]
    
    Return429 --> End([🏁 End])
    Allow --> End
    
    Note right of Start: <b>Sliding Window Algorithm</b><br/>Window: 1 minute<br/>Precision: per-second
```

---

## 6. CACHING STRATEGY

```mermaid
graph TB
    Start([🔍 Need Client Data]) --> CheckCache{💾 Check Redis Cache}
    
    CheckCache -->|✅ Cache Hit| ReturnCached[<b>✅ Return Cached Data</b><br/>~5ms response]
    
    CheckCache -->|❌ Cache Miss| QueryDB[🗄️ Query MongoDB<br/>~50ms response]
    
    QueryDB --> Found{Client Found?}
    
    Found -->|❌ No| ReturnNull[<b>❌ Return null</b>]
    
    Found -->|✅ Yes| StoreCache[💾 Store in Redis<br/>TTL: 5 minutes]
    
    StoreCache --> ReturnData[<b>✅ Return Client Data</b>]
    
    ReturnCached --> End([🏁 End])
    ReturnNull --> End
    ReturnData --> End
    
    Note right of Start: <b>Cache-Aside Pattern</b><br/>• Absolute TTL: 5 min<br/>• Sliding TTL: 2 min<br/>• Manual invalidation on update
```

---

## 7. CIRCUIT BREAKER STATE MACHINE

```mermaid
stateDiagram-v2
    [*] --> Closed: <b>Start</b>
    
    Closed --> Closed: <b>✅ Success</b><br/>Reset failure count
    Closed --> Open: <b>❌ 5 Failures</b><br/>Open circuit
    
    Open --> Open: <b>⏰ Timeout Not Expired</b><br/>Fail fast
    Open --> HalfOpen: <b>⏰ Timeout Expired</b><br/>Try recovery
    
    HalfOpen --> Open: <b>❌ Failure</b><br/>Re-open circuit
    HalfOpen --> Closed: <b>✅ Success</b><br/>Circuit recovered
    
    note right of Closed
        <b>CLOSED State</b>
        • Normal operation
        • All requests allowed
        • Tracking failures
    end note
    
    note right of Open
        <b>OPEN State</b>
        • Circuit tripped
        • Fail fast (no calls)
        • Wait for timeout
    end note
    
    note right of HalfOpen
        <b>HALF-OPEN State</b>
        • Testing recovery
        • Allow 1 request
        • Decide: open or closed
    end note
```

---

## 8. DATA MODELS

### Client Source Document (MongoDB)

```mermaid
classDiagram
    class ClientSource {
        +ObjectId _id
        +String clientId 🔑
        +String clientName
        +String apiKey 🔑
        +String apiSecret 🔒
        +Boolean isEnabled
        +DateTime validFrom
        +DateTime validTo
        +List~String~ allowedScopes
        +List~String~ allowedIPs
        +Int rateLimitPerMinute
        +DateTime createdAt
        +DateTime updatedAt
        +DateTime lastUsedAt
        +Object metadata
        
        +isValid() bool
        +isIPAllowed(ip) bool
        +hasScope(scope) bool
    }
    
    note for ClientSource "📦 Stored in MongoDB\n🔑 Unique indexes\n🔒 Encrypted at rest"
```

### JWT Token Structure

```mermaid
classDiagram
    class JwtToken {
        <<payload>>
        
        +String sub 🎯
        +String jti 🔑
        +Integer iat ⏰
        +Integer exp ⏰
        +String iss 🏢
        +String aud 🎯
        +List~String~ scopes 🔐
        +String client_name 📝
    }
    
    note for JwtToken "🎯 Subject = Client ID\n🔑 Unique Token ID\n⏰ Unix timestamps\n🏢 Issuer identifier\n🔐 Permissions"
```

### Token Pair Response

```mermaid
classDiagram
    class TokenPair {
        +String accessToken 🔑
        +String refreshToken 🔑
        +DateTime accessTokenExp ⏰
        +DateTime refreshTokenExp ⏰
        +String tokenType 💳
    }
    
    note for TokenPair "🔑 Bearer tokens\n⏰ UTC expiration\n💳 Always 'Bearer'"
```

---

## 9. SECURITY LAYERS

```mermaid
graph TB
    subgraph "🛡️ Security Layer 1 - Network"
        WAF[<b>🔥 WAF</b><br/>OWASP rules]
        DDoS[<b>⚡ DDoS Protection</b><br/>Edge filtering]
        SSL[<b>🔒 SSL/TLS</b><br/>HTTPS only]
    end
    
    subgraph "🛡️ Security Layer 2 - Gateway"
        RateLimit[<b>⚡ Rate Limiting</b><br/>Per-client throttling]
        CORS[<b>🔒 CORS Policy</b><br/>Domain whitelist]
        IPFilter[<b>🌐 IP Filtering</b><br/>Geo-blocking]
    end
    
    subgraph "🛡️ Security Layer 3 - Authentication"
        JWT[<b>🎫 JWT Validation</b><br/>Signature + claims]
        ClientCheck[<b>👤 Client Validation</b><br/>Active + dates]
        ScopeCheck[<b>🔐 Scope Verification</b><br/>Permissions]
    end
    
    subgraph "🛡️ Security Layer 4 - Authorization"
        RBAC[<b>👥 Role-Based Access</b>]
        ABAC[<b>📋 Attribute-Based</b>]
        AuditLog[<b>📝 Audit Logging</b>]
    end
    
    WAF --> DDoS
    DDoS --> SSL
    SSL --> RateLimit
    RateLimit --> CORS
    CORS --> IPFilter
    IPFilter --> JWT
    JWT --> ClientCheck
    ClientCheck --> ScopeCheck
    ScopeCheck --> RBAC
    RBAC --> ABAC
    ABAC --> AuditLog
```

---

## 10. ERROR HANDLING MATRIX

```mermaid
graph TB
    Start([⚠️ Error Occurs]) --> TypeCheck{Error Type?}
    
    TypeCheck -->|Auth Failed| AuthErr[<b>🔐 Authentication Error</b>]
    TypeCheck -->|AuthZ Failed| AuthZErr[<b>🔒 Authorization Error</b>]
    TypeCheck -->|Rate Limit| RateErr[<b>⚡ Rate Limit Error</b>]
    TypeCheck -->|Validation| ValErr[<b>📋 Validation Error</b>]
    TypeCheck -->|Server Error| SrvErr[<b>💥 Server Error</b>]
    
    AuthErr --> Return401[<b>↩️ 401 Unauthorized</b><br/>WWW-Authenticate: Bearer]
    AuthZErr --> Return403[<b>↩️ 403 Forbidden</b><br/>Insufficient scope]
    RateErr --> Return429[<b>↩️ 429 Too Many Requests</b><br/>Retry-After: 60]
    ValErr --> Return400[<b>↩️ 400 Bad Request</b><br/>Validation details]
    SrvErr --> Return500[<b>↩️ 500 Internal Error</b><br/>Generic message]
    
    Return401 --> Log[📝 Log Error Details]
    Return403 --> Log
    Return429 --> Log
    Return400 --> Log
    Return500 --> Log
    
    Log --> Alert[🚨 Alert if Critical]
    
    Alert --> Response([📤 Error Response])
    
    Note right of Start: <b>Error Response Format</b><br/>• Never expose stack traces<br/>• Include error code<br/>• Add correlation ID<br/>• Log full details server-side
```

---

## 11. MONITORING & OBSERVABILITY

```mermaid
graph TB
    subgraph "📊 Metrics Collection"
        APIMetrics[<b>📈 API Metrics</b><br/>RPS, latency, errors]
        AuthMetrics[<b>🔐 Auth Metrics</b><br/>Success/failure rate]
        CacheMetrics[<b>💾 Cache Metrics</b><br/>Hit/miss ratio]
        DBMetrics[<b>🗄️ DB Metrics</b><br/>Query time, connections]
    end
    
    subgraph "📝 Logging"
        AccessLog[<b>📋 Access Logs</b><br/>All requests]
        ErrorLog[<b>⚠️ Error Logs</b><br/>Exceptions & failures]
        AuditLog[<b>🔐 Audit Logs</b><br/>Security events]
    end
    
    subgraph "🔍 Distributed Tracing"
        TraceGen[<b>🔍 Trace Generator</b><br/>ActivitySource]
        SpanCollector[<b>📊 Span Collector</b><br/>OpenTelemetry]
        TraceBackend[<b>🗄️ Trace Storage</b><br/>Jaeger/Zipkin]
    end
    
    subgraph "📈 Dashboards & Alerts"
        Grafana[<b>📊 Grafana</b><br/>Metrics dashboards]
        Kibana[<b>🔍 Kibana</b><br/>Log analysis]
        PagerDuty[<b>🚨 PagerDuty</b><br/>Incident alerts]
    end
    
    APIMetrics --> Grafana
    AuthMetrics --> Grafana
    CacheMetrics --> Grafana
    DBMetrics --> Grafana
    
    AccessLog --> Kibana
    ErrorLog --> Kibana
    AuditLog --> Kibana
    
    TraceGen --> SpanCollector
    SpanCollector --> TraceBackend
    
    Grafana --> PagerDuty
    Kibana --> PagerDuty
```

---

## 12. DEPLOYMENT TOPOLOGY

```mermaid
graph TB
    subgraph "🌐 Internet"
        Users[<b>👥 Users</b>]
    end
    
    subgraph "☁️ Cloud Provider"
        CDN[<b>📦 CDN</b><br/>Static assets]
        WAF[<b>🔥 WAF</b><br/>Web firewall]
        LB[<b>⚖️ Load Balancer</b><br/>Traffic distribution]
    end
    
    subgraph "🖥️ Application Tier"
        API1[<b>🚪 API Instance #1</b>]
        API2[<b>🚪 API Instance #2</b>]
        API3[<b>🚪 API Instance #3</b>]
    end
    
    subgraph "🗄️ Data Tier"
        MongoPrimary[<b>📦 MongoDB Primary</b>]
        MongoSecondary[<b>📦 MongoDB Secondary</b>]
        RedisPrimary[<b>⚡ Redis Primary</b>]
        RedisReplica[<b>⚡ Redis Replica</b>]
    end
    
    Users --> CDN
    CDN --> WAF
    WAF --> LB
    
    LB --> API1
    LB --> API2
    LB --> API3
    
    API1 --> MongoPrimary
    API2 --> MongoPrimary
    API3 --> MongoPrimary
    
    API1 --> RedisPrimary
    API2 --> RedisPrimary
    API3 --> RedisPrimary
    
    MongoPrimary -.replication.-> MongoSecondary
    RedisPrimary -.replication.-> RedisReplica
```

---

## QUICK REFERENCE CHEATSHEET

### 🔑 Key Ports & Endpoints

| Service | Port | Endpoint | Purpose |
|---------|------|----------|---------|
| API | 7209 | `/` | Main API |
| API Health | 7209 | `/health` | Overall health |
| API Live | 7209 | `/health/live` | Liveness probe |
| API Ready | 7209 | `/health/ready` | Readiness probe |
| MongoDB | 27017 | - | Database |
| Redis | 6379 | - | Cache |

### 🎯 HTTP Status Codes

| Code | Meaning | When |
|------|---------|------|
| 200 | OK | Success |
| 400 | Bad Request | Invalid input |
| 401 | Unauthorized | Missing/invalid token |
| 403 | Forbidden | Insufficient permissions |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Error | Server error |

### 🔐 Required Headers

| Header | Required | Example |
|--------|----------|---------|
| `X-Client-Source` | ✅ Yes | `my-app` |
| `Token` | ✅ Yes | `eyJhbGc...` |
| `Refresh-Token` | ⚠️ Optional | `abc123...` |
| `Content-Type` | ⚠️ For POST/PUT | `application/json` |

---

**📅 Last Updated:** 2024-01-01  
**👤 Author:** Architecture Team  
**📋 Version:** 2.0
