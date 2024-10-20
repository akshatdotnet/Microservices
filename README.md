# Microservices

Microservices Architecture Overview

+--------------------------------------------------------+
|                    API Gateway                         |
|  - Routing, Load Balancing                             |
|  - Authentication, Authorization (JWT, OAuth)          |
|  - Rate Limiting, Caching                              |
+--------------------------------------------------------+
                     |
                     |
+---------------------+---------------------+-------------------+
|                     |                     |                   |
|                     |                     |                   |
|      +--------------v-------------+    +---v---------------+  +---v---------------+
|      |  Auth Service               |    |  User Service      |  |  File Service    |
|      |  (Handles Authentication)   |    |  (Manages Users)   |  |  (Manages Files) |
|      +----------------------------+    +--------------------+  +------------------+
|          |                        |        |                    |
|  +-------v--------+         +------v------+ |                    |
|  |  Token Service |         | Auth DB     | |                    |
|  +----------------+         +-------------+ |                    |
+------------------------------------------------+                  |
                     |                             |                |
                     |                             |                |
              +------v------+                +-----v------+   +-----v------+
              |  Logging    |                |   User DB   |   | File DB   |
              |  Service    |                +-------------+   +-----------+
              +-------------+
